using System.Threading.Tasks;

using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.WorkFlows;
using Magnett.Automation.Core.WorkFlows.Implementations;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Implementations.MockUps
{
    public class FlowRunnerBaseMock : FlowRunnerBase
    {
        private FlowRunnerBaseMock(IFlowDefinition definition, Context context) : base(definition, context)
        {
        }

        public override Task<NodeExit> Start()
        {
            throw new System.NotImplementedException();
        }
        
        public static IFlowRunner  Create(IFlowDefinition definition, Context context)
        {
            return new FlowRunnerBaseMock(definition, context);
        }
    }
}