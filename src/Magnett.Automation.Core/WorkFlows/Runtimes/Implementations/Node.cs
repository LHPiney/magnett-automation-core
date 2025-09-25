using System.Threading;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.WorkFlows.Runtimes.Events;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

public abstract class Node : NodeBase, INode
{
    protected Node(string name, IEventBus eventBus) : base(name, eventBus)
    {
            
    }
        
    protected Node(CommonNamedKey key, IEventBus eventBus) : base(key, eventBus)
    {
            
    }
    
    public Task<NodeExit> Execute(Context context)
    {
        return Execute(context, CancellationToken.None);
    }

    public async Task<NodeExit> Execute(Context context, CancellationToken cancellationToken)
    {
        NodeExit result = null;
        try
        {
            await TransitStateAsync(NodeAction.Execute, OnNodeExecuteEvent.Create(Key), cancellationToken);

            result = Handle(context);
        }
        catch (Exception ex)
        {
            result = NodeExit.Failed(ex.HResult.ToString(), ex.Message);
        }
        finally
        {
            if (result != null)
            { 
                result = await ManageResponse(result, cancellationToken);
            }
        }
        
        return result;
    }
    
    protected abstract NodeExit Handle(Context context);
}