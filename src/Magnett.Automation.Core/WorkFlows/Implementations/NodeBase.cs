namespace Magnett.Automation.Core.WorkFlows.Implementations;

public abstract class NodeBase
{
    public CommonNamedKey Key { get; }
        
    protected NodeBase(string name) : this(CommonNamedKey.Create(name))
    {
            
    }

    protected NodeBase(CommonNamedKey key)
    {
        Key = key 
              ?? throw new ArgumentNullException(nameof(key));
    }
}