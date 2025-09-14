#nullable enable
using System.Linq;
using System.Threading;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Runtimes.Factories;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

public abstract class FlowRunnerBase : IFlowRunner
{
    private readonly EventCollection _events = [];
    protected IFlowDefinition Definition { get; }
    protected INodeBase NodeToRun { get; set; }
    protected IEventBus? EventBus { get; }
    protected ILogger Logger { get; }
    
    public IReadOnlyCollection<IEvent> Events => _events
        .OrderBy<IEvent, DateTime>(e => e.CreatedAt)
        .ToList()
        .AsReadOnly();

    protected FlowRunnerBase(
        IFlowDefinition definition, 
        Context context, 
        IEventBus? eventBus,
        ILogger<FlowRunnerBase> logger)
    {
        Definition = definition 
                     ?? throw new ArgumentNullException(nameof(definition));

        Context = context 
                      ?? throw new  ArgumentNullException(nameof(context));

        EventBus = eventBus;

        Logger = logger
                 ?? throw new ArgumentNullException(nameof(logger));
        
        NodeToRun = InflateNodeDefinition(Definition.InitialNode);
    }

    private INodeBase InflateNodeDefinition(CommonNamedKey nodeName)
    {
        return InflateNodeDefinition(Definition.GetNode(nodeName));
    }
    
    protected virtual INodeBase InflateNodeDefinition(INodeDefinition nodeDefinition)
    {
        return NodeFactory.CreateNode(nodeDefinition, EventBus);
    }
    
    protected virtual async Task PublishEventAsync(IEvent @event)
    {
        _events.Add(@event);
        await EventBus?.PublishAsync(@event)!;
    }

    public Context Context { get; }

    protected async Task<NodeExit> ExecuteNodeAsync(INodeBase node, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Executing node [{NodeName}]", node.Name);
        
        if (!node.IsInitialized || node.State == NodeState.Idle) 
        {
            Logger.LogDebug("Node [{NodeName}] initialized", node.Name);
            
            await node.Init(cancellationToken);
        }
        
        return node switch
        {
            INodeAsync nodeAsync => await nodeAsync.ExecuteAsync(Context, cancellationToken),
            INode      nodeSync  => await nodeSync.Execute(Context, cancellationToken),
            _                    => throw new ArgumentOutOfRangeException(nameof(node), node, null)
        };
    }

    public abstract Task<NodeExit> Start(CancellationToken cancellationToken = default);
}