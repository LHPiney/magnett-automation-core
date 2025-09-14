using System.Threading;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.WorkFlows.Runtimes.Events;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

public abstract class NodeAsync : NodeBase, INodeAsync
{
    protected NodeAsync(string name, IEventBus eventBus) : base(name, eventBus)
    {
            
    }
        
    protected NodeAsync(CommonNamedKey key, IEventBus eventBus) : base(key, eventBus)
    {
            
    }

    public async Task<NodeExit> ExecuteAsync(Context context, CancellationToken cancellationToken)
    {
        NodeExit result = null;
        try
        {
            await TransitStateAsync(NodeAction.Execute, OnNodeExecuteEvent.Create(Key), cancellationToken);
            
            result = await HandleAsync(context, cancellationToken);
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
    
    protected abstract Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default);
}