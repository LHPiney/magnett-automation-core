using System.Threading;
using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.Events;

/// <summary>
/// Defines the contract for an event bus that allows asynchronous event publishing and provides an event stream.
/// </summary>
public interface IEventBus:  IAsyncDisposable
{
    public EventBusState State { get; }
    public bool IsReady => State == EventBusState.Ready;
    public Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default);
    public IEventConsumerRegistry ConsumerRegistry { get; }
    public IEventReader EventReader { get; }
    public IEventBus Start(CancellationToken cancellationToken = default);
    public Task StopAsync(CancellationToken cancellationToken = default);
}