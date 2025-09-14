using System.Linq;
using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.Events.Implementations;

/// <summary>
/// Manages the publication and dispatching of events to their respective consumers
/// in an asynchronous and non-blocking manner. It also provides an optional
/// event stream for persistence.
/// </summary>
public sealed class EventBus : IEventBus
{
    private readonly ILogger<EventBus> _logger;
    private readonly Channel<IEvent> _eventChannel;
    private readonly IEventStream? _eventStream;
    private readonly object _stateLock = new();

    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _processingTask;

    public EventBusState State { get; private set; }    
    public IEventConsumerRegistry ConsumerRegistry { get; }
    public IEventReader EventReader => _eventStream;

    private EventBus(
        IEventConsumerRegistry consumerRegistry,
        ILogger<EventBus> logger,
        IEventStream? eventStream = null)
    {
        ConsumerRegistry = consumerRegistry ?? throw new ArgumentNullException(nameof(consumerRegistry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _eventStream = eventStream;
        _eventChannel = Channel.CreateUnbounded<IEvent>();
        State = EventBusState.Stopped;
    }

    public IEventBus Start(CancellationToken cancellationToken = default)
    {
        lock (_stateLock)
        {
            if (State == EventBusState.Ready)
            {
                _logger.LogWarning("EventBus is already running");
                return  this;
            }

            _logger.LogInformation("EventBus is starting...");
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _processingTask = Task.Run(() => ProcessingTask(_cancellationTokenSource.Token), cancellationToken);
            State = EventBusState.Ready;
            _logger.LogInformation("EventBus has started and is ready to process events");

            return this;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default) 
    {
        lock (_stateLock)
        {
            if (State == EventBusState.Stopped)
            {
                _logger.LogWarning("EventBus is already stopped");
                return;
            }

            _logger.LogInformation("EventBus is stopping...");
            if (_cancellationTokenSource is { IsCancellationRequested: false })
            {
                _cancellationTokenSource.Cancel();
            }
            
            State = EventBusState.Stopped;
        }

        if (_processingTask != null)
        {
            try
            {
                await _processingTask.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("StopAsync was canceled while waiting for processing task to end");
            }
        }

        _logger.LogInformation("EventBus has been stopped");
    } 
    public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);
        if (cancellationToken.IsCancellationRequested) return;

        if (State != EventBusState.Ready)
        {
            throw new InvalidOperationException("EventBus is not running. Please start it before publishing events.");
        } 

        try
        {
            await _eventChannel.Writer.WriteAsync(@event, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Publishing event of type {EventType} was canceled", @event.GetType().Name);
        }
    }

    private async Task ProcessingTask(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event processing task started");
        try
        {
            await foreach (var @event in _eventChannel.Reader.ReadAllAsync(cancellationToken))
            {
                await ProcessEvent(@event, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Event processing task was gracefully canceled");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "A critical error occurred in the event processing task");
        }
        finally
        {
            _logger.LogInformation("Event processing task finished");
        }
    }   

    private async Task ProcessEvent(IEvent @event, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(@event);  

        var consumers = ConsumerRegistry.GetConsumers(@event.GetType());
        var consumerTasks = consumers.Select(consumer =>
        {
            try
            {
                return consumer.HandleEventAsync(@event, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Event consumer {ConsumerType} failed while handling {EventType}",
                    consumer.GetType().Name, @event.GetType().Name);
                return Task.CompletedTask;
            }
        });
        await Task.WhenAll(consumerTasks);

        if (_eventStream != null)
        {
            try
            {
                await _eventStream.Writer.WriteAsync(@event, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write event {EventType} to the event stream", @event.GetType().Name);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (State != EventBusState.Stopped)
        { 
            await StopAsync();
        }
        _cancellationTokenSource?.Dispose();
        _processingTask?.Dispose();
    }

    public static IEventBus Create(
        ILogger<EventBus> logger,
        IEventStream? eventStream = null,
        bool startOnCreate = true)
    {
        ArgumentNullException.ThrowIfNull(logger);

        var consumerRegistry = new EventConsumerRegistry();

        return Create(consumerRegistry, logger, eventStream, startOnCreate);
    }


    public static IEventBus Create(
        IEventConsumerRegistry consumerRegistry,
        ILogger<EventBus> logger,
        IEventStream? eventStream = null,
        bool startOnCreate = true)
    {
        ArgumentNullException.ThrowIfNull(consumerRegistry);
        ArgumentNullException.ThrowIfNull(logger);

        var eventStreamToUse = eventStream ?? new EventStream(logger);

        return startOnCreate 
            ? new EventBus(consumerRegistry, logger, eventStreamToUse).Start()
            : new EventBus(consumerRegistry, logger, eventStreamToUse);
    }   
}