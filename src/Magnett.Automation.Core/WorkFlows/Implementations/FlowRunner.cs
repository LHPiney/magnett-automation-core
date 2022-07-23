namespace Magnett.Automation.Core.WorkFlows.Implementations;

public sealed class FlowRunner : FlowRunnerBase
{
    private FlowRunner(IFlowDefinition definition, Context context) : base(definition, context)
    {
    }

    #region IFlowRunner
    
    public override async Task<NodeExit> Start()
    {
        var isFlowFinished = false;
        NodeExit nodeExit;
            
        do
        {
            nodeExit = await ExecuteNode(NodeToRun);
                
            NodeToRun = Definition.GetNode(NodeToRun, nodeExit.Code);

            isFlowFinished = (NodeToRun == null);

        } while (!isFlowFinished);

        return nodeExit;
    }
        
    #endregion
        
    public static IFlowRunner  Create(IFlowDefinition definition, Context context)
    {
        return new FlowRunner(definition, context);
    }
}