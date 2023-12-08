using Magnett.Automation.Core.WorkFlows.Definitions;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

public sealed class FlowRunner : FlowRunnerBase
{
    private FlowRunner(
        IFlowDefinition definition, 
        Context context, 
        ILogger<FlowRunner> logger) : base(definition, context, logger)
    {
    }

    #region IFlowRunner
    
    public override async Task<NodeExit> Start()
    {
        bool isFlowFinished;
        NodeExit nodeExit;
            
        do
        {
            nodeExit = await ExecuteNode(NodeToRun);
            NodeToRun = NextNode(NodeToRun, nodeExit);
            isFlowFinished = (NodeToRun is null);

        } while (!isFlowFinished);

        return nodeExit;

        INodeBase NextNode(INodeBase currentNode, NodeExit nodeExit)
        {
            var nextNodeDefinition = Definition
                .GetNode(NodeToRun, nodeExit.Code);
            
            return nextNodeDefinition is null 
                ? null 
                : InflateNodeDefinition(nextNodeDefinition);
        }
    }
        
    #endregion
        
    public static IFlowRunner  Create(
        IFlowDefinition definition, 
        Context context, 
        ILogger<FlowRunner> logger = null)
    {
        logger ??= LoggerFactory
            .Create(builder => builder.AddConsole(opt => 
                opt.LogToStandardErrorThreshold = LogLevel.Information))
            .CreateLogger<FlowRunner>();

        return new FlowRunner(definition, context, logger);
    }
}