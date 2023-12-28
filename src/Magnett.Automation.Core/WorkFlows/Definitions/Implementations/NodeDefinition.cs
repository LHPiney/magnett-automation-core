namespace Magnett.Automation.Core.WorkFlows.Definitions.Implementations;

public class NodeDefinition : INodeDefinition
{
    public CommonNamedKey Key { get; private init; }
    public Type NodeType { get; private init; }

    public static NodeDefinition Create<TNodeType>(CommonNamedKey nodeName)
    {
        return new NodeDefinition
        {
            Key = nodeName,
            NodeType = typeof(TNodeType)
        };
    }
}