#nullable enable
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Magnett.Automation.Core.Events.Implementations;

/// <summary>
/// Base class for entities that emit events for state changes.
/// The actual state changes are handled by event handlers, not by this entity.
/// </summary>
public abstract class EventEmitterEntity(IEventBus? eventBus)
{
    private readonly ConcurrentQueue<IEvent> _eventStore = new();

    protected IEventBus? EventBus => eventBus;

    /// <summary>
    /// Emits an event for state change. The actual state change will be handled by event handlers.
    /// </summary>
    protected async Task EmitEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        ArgumentNullException.ThrowIfNull(@event);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        
        _eventStore.Enqueue(@event);
        
        if (eventBus != null)
        {
            await eventBus.PublishAsync(@event, cancellationToken);
        }
    }

    /// <summary>
    /// Gets all events emitted by this entity in chronological order.
    /// </summary>
    public IReadOnlyCollection<IEvent> LocalEvents => 
        _eventStore.OrderBy(e => e.CreatedAt).ToList().AsReadOnly();

    /// <summary>
    /// Clears local events (useful for testing or when events are persisted).
    /// </summary>
    protected void ClearLocalEvents()
    {
        while (_eventStore.TryDequeue(out _)) { }
    }
}