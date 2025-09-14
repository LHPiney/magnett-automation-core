using System.Threading;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.Events.Implementations;

public class EventStreamConsumer : IAsyncDisposable
{
    private readonly IEventReader _eventReader;
    private readonly ILogger _logger;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private bool _disposed;

    public EventStreamConsumer(IEventReader eventReader, ILogger logger)
    {
        _eventReader = eventReader;
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(EventStreamConsumer));
        
        using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(
            _cancellationTokenSource.Token, cancellationToken);
            
        await foreach (var @event in _eventReader.Reader.ReadAllAsync(combinedCts.Token))
        {
            try
            {
                await ProcessEventAsync(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event {EventId}", @event.Id);
            }
        }
    }

    private async Task ProcessEventAsync(IEvent @event)
    {
        _logger.LogInformation("Processing event {EventName} with ID {EventId}", 
            @event.Name, @event.Id);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        
        _disposed = true;
        
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        
        if (_eventReader != null)
        {
            await _eventReader.DisposeAsync();
        }
        
        GC.SuppressFinalize(this);
    }
} 