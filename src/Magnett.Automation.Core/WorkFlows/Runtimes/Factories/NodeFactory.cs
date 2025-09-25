using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.WorkFlows.Definitions;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Factories;

public static class NodeFactory
{
    public static INodeBase CreateNode(INodeDefinition nodeDefinition, IEventBus eventBus)
    {
        ArgumentNullException.ThrowIfNull(nodeDefinition);

        var constructor = nodeDefinition.NodeType
            .GetConstructor([typeof(CommonNamedKey), typeof(IEventBus)]);

        if (constructor is null)
        {
            throw new InvalidOperationException($"Type {nodeDefinition.NodeType.FullName} does not have valid node constructor");
        }
        
        return (INodeBase)constructor
            .Invoke([nodeDefinition.Key, eventBus]);
    }
}