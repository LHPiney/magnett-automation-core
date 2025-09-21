using System.Linq;
using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Magnett.Automation.Core.Metrics;
using Magnett.Automation.Core.Events.Metrics;

using DefaultHandlerRegistry = Magnett.Automation.Core.Events.Implementations.EventHandlerRegistry;
using Magnett.Automation.Core.Metrics.Implementations;

namespace Magnett.Automation.Core.Events.Implementations;

/// <summary>
/// Manages the publication and dispatching of events to their respective consumers
/// in an asynchronous and non-blocking manner. It also provides an optional
/// event stream for persistence.
/// </summary>
public sealed class EventBus : IEventBus
{
    private readonly ILogger<EventBus> _logger;
    private readonly Channel<EventContext> _eventChannel;
    private readonly IEventStream? _eventStream;
    private readonly IMetricsCollector? _metricsCollector;
    private readonly object _stateLock = new();
    private bool _disposed = false;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _processingTask;
    public EventBusState State { get; private set; }    
    public IEventHandlerRegistry EventHandlerRegistry { get; }
    public IEventReader EventReader => _eventStream;
    public int QueueSize => _eventChannel.Reader.CanCount ? _eventChannel.Reader.Count : 0;
    public IMetricsRegistry? MetricsRegistry => _metricsCollector is MetricsCollector collector ? collector.Registry : null;

    private EventBus(
        IEventHandlerRegistry eventHandlerRegistry,
        ILogger<EventBus> logger,
        IEventStream? eventStream = null,
        IMetricsCollector? metricsCollector = null)
    {
        EventHandlerRegistry = eventHandlerRegistry ?? throw new ArgumentNullException(nameof(eventHandlerRegistry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _eventStream = eventStream;
        _metricsCollector = metricsCollector;
        _eventChannel = Channel.CreateUnbounded<EventContext>();

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

            using (_logger.BeginScope("EventBus Start"))
            {
                _logger.LogInformation("EventBus is starting...");
                _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                _processingTask = Task.Run(() => ProcessingTask(_cancellationTokenSource.Token), cancellationToken);
                State = EventBusState.Ready;
                
                _metricsCollector?.Set(new QueueSize("eventbus"), QueueSize);
                
                _logger.LogInformation("EventBus has started and is ready to process events");
            }

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

            using (_logger.BeginScope("EventBus Stop"))
            {

                _logger.LogInformation("EventBus is stopping...");
                if (_cancellationTokenSource is { IsCancellationRequested: false })
                {
                    _cancellationTokenSource.Cancel();
                }
                State = EventBusState.Stopped;
                _logger.LogInformation("EventBus has been stopped");
            }
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
        
        if (_eventStream != null)
        {
            try
            {
                _eventStream.Writer.Complete();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to complete EventStream writer during stop");
            }
        }
    } 
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IEvent
    {
        if (_disposed) throw new ObjectDisposedException(nameof(EventBus));
        
        ArgumentNullException.ThrowIfNull(@event);

        if (cancellationToken.IsCancellationRequested) return;

        if (State != EventBusState.Ready)
        {
            throw new InvalidOperationException("EventBus is not running. Please start it before publishing events.");
        } 

        try
        {
            _metricsCollector?.Increment(new EventsPublished(@event.GetType().Name));
            
            var processor = CreateTypedProcessor<TEvent>();
            var context = new EventContext(@event, processor);
            _logger.LogDebug("Writing event context {EventType} to channel", @event.GetType().Name);
            await _eventChannel.Writer.WriteAsync(context, cancellationToken);
            _logger.LogDebug("Successfully wrote event context {EventType} to channel", @event.GetType().Name);
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
            _logger.LogDebug("Starting to read from event channel");
            await foreach (var context in _eventChannel.Reader.ReadAllAsync(cancellationToken))
            {
                await ProcessEventContext(context, cancellationToken);
            }
            _logger.LogInformation("Event channel reader completed - no more events to process");
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

    private async Task ProcessEventContext(EventContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        var startTime = DateTime.UtcNow;
        var success = false;
        
        if (_eventStream != null)
        {
            try
            {
                await _eventStream.Writer.WriteAsync(context.Event, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write event {EventType} to the event stream", context.EventType.Name);
            }
        }
        
        try
        {
            await context.ProcessAsync(cancellationToken);
            success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process event context {EventType}", context.EventType.Name);
        }
        
        var processingTime = DateTime.UtcNow - startTime;
        
        if (_metricsCollector != null)
        {
            if (success)
            {
                _metricsCollector.Increment(new EventsProcessed(context.EventType.Name));
            }
            else
            {
                _metricsCollector.Increment(new EventsFailed(context.EventType.Name));
            }
            
            _metricsCollector.Record(new ProcessingTime(context.EventType.Name), processingTime.TotalMilliseconds);
        }
    }

    private Func<EventContext, CancellationToken, Task> CreateTypedProcessor<TEvent>() 
        where TEvent : IEvent
    {
        return async (context, cancellationToken) =>
        {
            var typedEvent = (TEvent)context.Event;
            var handlers = EventHandlerRegistry.GetEventHandlers(typedEvent);
            var handlerTasks = handlers.Select(handler =>
            {
                try
                {
                    return handler.Handle(typedEvent, _logger, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Event handler {HandlerType} failed while handling {EventType}",
                        handler.GetType().Name, typedEvent.GetType().Name);
                    return Task.CompletedTask;
                }
            });
            await Task.WhenAll(handlerTasks);
        };
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        try
        {
            if (State != EventBusState.Stopped)
            {
                _cancellationTokenSource?.Cancel();
            }
            
            _cancellationTokenSource?.Dispose();
            _processingTask?.Dispose();
            _eventChannel.Writer.Complete();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error occurred during EventBus disposal");
        }
        finally
        {
            _disposed = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        
        try
        {
            if (State != EventBusState.Stopped)
            { 
                await StopAsync();
            }
            
            _cancellationTokenSource?.Dispose();
            _processingTask?.Dispose();
            _eventChannel.Writer.Complete();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error occurred during EventBus async disposal");
        }
        finally
        {
            _disposed = true;
        }
    }

    /// <summary>
    /// Creates a new EventBus instance with the provided logger. The event bus will be started automatically.
    /// </summary>
    /// <param name="logger">The logger to use for logging.</param>
    /// <returns>A new EventBus instance.</returns>
    public static IEventBus Create(ILogger<EventBus> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        return EventBus.Create(
            logger, 
            null, 
            null,
            true);
    }

    /// <summary>
    /// Creates a new EventBus instance with the provided logger, event stream, and metrics collector.
    /// </summary>
    /// <param name="logger">The logger to use for logging.</param>
    /// <param name="eventStream">The event stream to use for persisting events.</param>
    /// <param name="metricsCollector">The metrics collector to use for collecting metrics.</param>
    /// <param name="startOnCreate">Whether to start the event bus automatically.</param>
    /// <returns>A new EventBus instance.</returns>
    public static IEventBus Create(
        ILogger<EventBus> logger,
        IEventStream? eventStream = null,
        IMetricsCollector? metricsCollector = null,
        bool startOnCreate = true)
    {
        ArgumentNullException.ThrowIfNull(logger);

        var handlerRegistry = DefaultHandlerRegistry.Create(logger);

        return Create(logger, handlerRegistry, eventStream, metricsCollector, startOnCreate);
    }

    /// <summary>
    /// Creates a new EventBus instance with the provided logger, handler registry, event stream, and metrics collector.
    /// </summary>
    /// <param name="logger">The logger to use for logging.</param>
    /// <param name="handlerRegistry">The handler registry to use for event handling.</param>
    /// <param name="eventStream">The event stream to use for persisting events.</param>
    /// <param name="metricsCollector">The metrics collector to use for collecting metrics.</param>
    /// <param name="startOnCreate">Whether to start the event bus automatically.</param>
    /// <returns>A new EventBus instance.</returns>
    public static IEventBus Create(
        ILogger<EventBus> logger,
        IEventHandlerRegistry handlerRegistry,
        IEventStream? eventStream = null,
        IMetricsCollector? metricsCollector = null,
        bool startOnCreate = true)
    {
        ArgumentNullException.ThrowIfNull(handlerRegistry);
        ArgumentNullException.ThrowIfNull(logger);

        var eventStreamToUse = eventStream ?? EventStream.Create(logger);
        var metricsCollectorToUse = metricsCollector ?? MetricsCollector.Create();

        return startOnCreate 
            ? new EventBus(handlerRegistry, logger, eventStreamToUse, metricsCollectorToUse).Start()
            : new EventBus(handlerRegistry, logger, eventStreamToUse, metricsCollectorToUse);
    }   
}