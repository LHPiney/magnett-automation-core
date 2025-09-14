using System.Threading;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

public sealed class FlowRunner : FlowRunnerBase
{
    private FlowRunner(
        IFlowDefinition definition, 
        Context context, 
        IEventBus eventBus,
        ILogger<FlowRunner> logger) : base(definition, context, eventBus, logger)
    {
    }

    #region IFlowRunner
    
    public override async Task<NodeExit> Start(CancellationToken cancellationToken = default)
    {
        bool isFlowFinished;
        NodeExit nodeExit;
            
        do
        {
            nodeExit = await ExecuteNodeAsync(NodeToRun, cancellationToken);
            NodeToRun = NextNode();
            isFlowFinished = (NodeToRun is null);

        } while (!isFlowFinished);

        return nodeExit;

        INodeBase NextNode()
        {
            var nextNodeDefinition = Definition
                .GetNode(NodeToRun.Key, nodeExit.Code);
            
            return nextNodeDefinition is null 
                ? null 
                : InflateNodeDefinition(nextNodeDefinition);
        }
    }
        
    #endregion
        
    public static IFlowRunner  Create(
        IFlowDefinition definition, 
        Context context, 
        IEventBus eventBus = null,
        ILogger<FlowRunner> logger = null)
    {
        logger ??= LoggerFactory    
            .Create(builder => builder.AddConsole(opt => 
                opt.LogToStandardErrorThreshold = LogLevel.Information))
            .CreateLogger<FlowRunner>();

        if (eventBus is not null) return new FlowRunner(definition, context, eventBus, logger);
        {
            var eventbusLogger = LoggerFactory
                .Create(builder => builder.AddConsole(opt => 
                    opt.LogToStandardErrorThreshold = LogLevel.Information))
                .CreateLogger<EventBus>();

            eventBus = Core.Events.Implementations.EventBus.Create(eventbusLogger);
        }

        return new FlowRunner(definition, context, eventBus, logger);
    }
}