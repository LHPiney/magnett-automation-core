using System.Threading.Tasks;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.WorkFlows;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;

public class FlowRunnerBaseFake : FlowRunnerBase
{
    public FlowRunnerBaseFake(IFlowDefinition definition, Context context) : base(definition, context)
    {
        
    }

    public override async Task<NodeExit> Start()
    {
        return await ExecuteNode(NodeToRun);
    }
}