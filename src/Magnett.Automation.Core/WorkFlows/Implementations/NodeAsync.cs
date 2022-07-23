namespace Magnett.Automation.Core.WorkFlows.Implementations;

public abstract class NodeAsync : NodeBase, INodeAsync
{
    protected NodeAsync(string name) : base(name)
    {
            
    }
        
    protected NodeAsync(CommonNamedKey key) : base(key)
    {
            
    }

    public abstract Task<NodeExit> Execute(Context context);
}