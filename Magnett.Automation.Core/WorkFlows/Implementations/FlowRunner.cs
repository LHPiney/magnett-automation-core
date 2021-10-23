using System.Threading.Tasks;
using Magnett.Automation.Core.Contexts;

namespace Magnett.Automation.Core.WorkFlows.Implementations
{
    public class FlowRunner : FlowRunnerBase
    {
        private FlowRunner(IFlowDefinition definition, Context context) : base(definition, context)
        {
        }

        #region IFlowRunner

        public override async Task Start()
        {
            var isFlowFinished = false;
            
            do
            {
                var nodeExit = await ExecuteNode(NodeToRun);
                
                NodeToRun = Definition.GetNode(NodeToRun, nodeExit.Code);

                isFlowFinished = (NodeToRun == null);

            } while (!isFlowFinished);
        }
        
        #endregion

        public static IFlowRunner  Create(IFlowDefinition definition, Context context)
        {
            return new FlowRunner(definition, context);
        }
    }
}