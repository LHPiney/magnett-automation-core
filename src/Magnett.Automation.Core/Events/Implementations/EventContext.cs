using System.Threading;

namespace Magnett.Automation.Core.Events.Implementations;

internal sealed class EventContext
{
    public IEvent Event { get; }
    public Type EventType { get; }
    public Func<EventContext, CancellationToken, Task> Processor { get; }
    public DateTime CreatedAt { get; }
    
    public EventContext(IEvent @event, Func<EventContext, CancellationToken, Task> processor)
    {
        Event = @event ?? throw new ArgumentNullException(nameof(@event));
        Processor = processor ?? throw new ArgumentNullException(nameof(processor));
        EventType = @event.GetType();
        CreatedAt = DateTime.UtcNow;
    }
    
    public Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        return Processor(this, cancellationToken);
    }
    
    public override string ToString()
    {
        return $"EventContext: {EventType.Name} - {Event.Name} - CreatedAt: {CreatedAt:yyyy-MM-dd HH:mm:ss}";
    }
}
