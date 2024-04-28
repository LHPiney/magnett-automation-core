using System.Threading.Tasks;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;

public class FlowRunnerBaseFake : FlowRunnerBase
{
    public FlowRunnerBaseFake(
        IFlowDefinition definition, 
        Context context,
        ILogger<FlowRunnerBaseFake> logger) : base(definition, context, logger)
    {
        
    }

    public override async Task<NodeExit> Start()
    {
        return await ExecuteNode(NodeToRun);
    }
}