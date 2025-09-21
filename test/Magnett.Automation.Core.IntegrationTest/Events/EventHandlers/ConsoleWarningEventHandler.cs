using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.IntegrationTest.Events.Events;
using Magnett.Automation.Core.Events;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.IntegrationTest.Events.EventHandlers;

public class ConsoleWarningEventHandler : IEventHandler<TestEvent>
{
    public async Task Handle(TestEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        logger?.LogWarning("Warning Event handled: {@event}", @event);
        await Task.Delay(10, cancellationToken);
    }
}
