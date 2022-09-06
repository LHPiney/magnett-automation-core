namespace Magnett.Automation.Core.WorkFlows.Definitions.Implementations;

public class NodeDefinition : INodeDefinition
{
    public Type Type { get; }

    public string Name { get; }
    
    public bool IsSingleton { get; }

    private NodeDefinition(Type type, string name, bool isSingleton = true)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException(null, nameof(name));
        
        Type = type ?? 
                throw new ArgumentException(null, nameof(type));
        Name = name;
        IsSingleton = isSingleton;
    }
    
    public static INodeDefinition Create<T>(string name, bool isSingleton = false)
    {
        return new NodeDefinition(typeof(T), name, isSingleton);
    }
}