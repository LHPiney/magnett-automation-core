using Magnett.Automation.Core.WorkFlows.Definitions.Collections;
using Magnett.Automation.Core.WorkFlows.Definitions.Implementations;
using Magnett.Automation.Core.WorkFlows.Runtimes;

namespace Magnett.Automation.Core.WorkFlows.Definitions.Builders;

public class FlowDefinitionBuilder
{
    private readonly NodeLinkList       _links;
    private readonly NodeDefinitionList _nodes;
        
    private CommonNamedKey _initialNode;

    private FlowDefinitionBuilder()
    {
        _links = NodeLinkList.Create();
        _nodes = NodeDefinitionList.Create();
    }

    private INodeLinkBuilder StoreNodeLink(
        CommonNamedKey sourceNode,
        INodeLink nodeLink)
    {
        _links.Add(nodeLink.Key, nodeLink);

        return NodeLinkBuilder.Create(
            sourceNode, 
            StoreNodeLink, 
            () => this);
    }

    public INodeLinkBuilder WithInitialNode<TNodeType>(CommonNamedKey nodeName)
    {
        _initialNode = nodeName;

        return WithNode<TNodeType>(nodeName);
    }

    public INodeLinkBuilder WithNode<TNodeType>(CommonNamedKey nodeName)
    {
        _nodes.Add(
            nodeName,
            NodeDefinition.Create<TNodeType>(nodeName));

        return NodeLinkBuilder.Create(
            nodeName, 
            StoreNodeLink, 
            () => this);
    }

    public IFlowDefinition BuildDefinition()
    {
        return FlowDefinition.Create(_initialNode, _nodes, _links);
    }

    public static FlowDefinitionBuilder Create()
    {
        return new FlowDefinitionBuilder();
    }
}