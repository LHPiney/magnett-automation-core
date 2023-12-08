using Magnett.Automation.Core.WorkFlows.Definitions;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Factories;

public static class NodeFactory
{
    public static INodeBase CreateNode(INodeDefinition nodeDefinition)
    {
        ArgumentNullException.ThrowIfNull(nodeDefinition);

        var constructor = nodeDefinition.NodeType
            .GetConstructor(new[] { typeof(CommonNamedKey) });

        if (constructor is null)
        {
            throw new InvalidOperationException($"Type {nodeDefinition.NodeType.FullName} does not have valid node constructor");
        }
        
        return (INodeBase)constructor
            .Invoke(new object[] { nodeDefinition.Key });
    }
}