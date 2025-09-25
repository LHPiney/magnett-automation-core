using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;

public class FlowRunnerBaseFake(
    IFlowDefinition definition,
    Context context,
    IEventBus eventBus,
    ILogger<FlowRunnerBaseFake> logger) : FlowRunnerBase(definition, context, eventBus, logger)
{
    public override async Task<NodeExit> Start(CancellationToken cancellationToken = default)
    {
        return await ExecuteNodeAsync(NodeToRun, cancellationToken);
    }
}