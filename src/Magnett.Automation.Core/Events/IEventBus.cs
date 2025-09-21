using System.Threading;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.Metrics;

namespace Magnett.Automation.Core.Events;

/// <summary>
/// Defines the contract for an event bus that allows asynchronous event publishing and provides an event stream.
/// </summary>
public interface IEventBus: IAsyncDisposable, IDisposable
{
    public EventBusState State { get; }
    public bool IsReady => State == EventBusState.Ready;
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
    public IEventHandlerRegistry EventHandlerRegistry { get; }
    public IEventReader EventReader { get; }
    public IMetricsRegistry? MetricsRegistry { get; }
    public IEventBus Start(CancellationToken cancellationToken = default);
    public Task StopAsync(CancellationToken cancellationToken = default);
}