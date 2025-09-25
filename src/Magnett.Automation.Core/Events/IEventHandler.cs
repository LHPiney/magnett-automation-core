using System.Threading;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.Events;

public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    Task Handle(TEvent @event, ILogger logger, CancellationToken cancellationToken = default);
}