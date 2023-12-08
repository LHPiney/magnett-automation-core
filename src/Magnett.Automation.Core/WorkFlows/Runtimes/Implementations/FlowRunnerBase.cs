using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Runtimes.Factories;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

public abstract class FlowRunnerBase : IFlowRunner
{
    protected IFlowDefinition Definition { get; }
    protected INodeBase NodeToRun { get; set; }
    protected ILogger Logger { get; }
    
    protected FlowRunnerBase(
        IFlowDefinition definition, 
        Context context, 
        ILogger<FlowRunnerBase> logger)
    {
        Definition = definition 
                     ?? throw new ArgumentNullException(nameof(definition));

        Context = context 
                      ?? throw new  ArgumentNullException(nameof(context));
            
        NodeToRun = InflateNodeDefinition(Definition.InitialNode);
        
        Logger = logger 
                 ?? throw new ArgumentNullException(nameof(logger));
    }

    private INodeBase InflateNodeDefinition(CommonNamedKey nodeName)
    {
        return InflateNodeDefinition(Definition.GetNode(nodeName));
    }
    
    protected INodeBase InflateNodeDefinition(INodeDefinition nodeDefinition)
    {
        return NodeFactory
            .CreateNode(nodeDefinition);
    }

    public Context Context { get; }

    protected async Task<NodeExit> ExecuteNode(INodeBase node)
    {
        Logger.LogDebug($"Executing node [{node.Name}]");
        
        return node switch
        {
            INodeAsync nodeAsync => await nodeAsync.Execute(Context),
            INode      nodeSync  => await Task.Run(() => nodeSync.Execute(Context)),
            null                 => throw new ArgumentNullException(nameof(node)),
            _                    => throw new ArgumentOutOfRangeException(nameof(node), node, null)
        };
    }

    public abstract Task<NodeExit> Start();
}