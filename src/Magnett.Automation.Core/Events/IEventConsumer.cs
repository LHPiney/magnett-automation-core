using System.Threading;

namespace Magnett.Automation.Core.Events;

public interface IEventConsumer
{
    Guid ConsumerId { get; }
    Task HandleEventAsync(IEvent @event, CancellationToken cancellationToken = default);
} 