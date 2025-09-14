using Magnett.Automation.Core.WorkFlows.Definitions.Collections;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

[assembly: InternalsVisibleTo("Magnett.Automation.Core.UnitTest")]
namespace Magnett.Automation.Core.WorkFlows.Definitions.Implementations;
internal class  FlowDefinition : IFlowDefinition
{
    private readonly NodeDefinitionList _nodes;
    private readonly NodeLinkList       _links;
        
    private FlowDefinition(
        CommonNamedKey     initialNode,
        NodeDefinitionList nodes,
        NodeLinkList       links)
    {
        InitialNode = initialNode
                      ?? throw new ArgumentNullException(nameof(initialNode));
        _nodes = nodes 
                 ?? throw new ArgumentNullException(nameof(nodes));
        _links = links
                 ?? throw new ArgumentNullException(nameof(links));
    }

    #region IFlowDefinition

    public CommonNamedKey InitialNode { get; }

    public bool HasNode(CommonNamedKey nodeKey)
    {
        return _nodes.HasItem(nodeKey);
    }
       
    public INodeDefinition GetNode(CommonNamedKey nodeKey)
    {
        return _nodes.Get(nodeKey);
    }

    public INodeDefinition GetNode(CommonNamedKey key, string code)
    {
        INodeDefinition result = null;

        var link = HasLink(key, code)
            ? GetLink(key, code)
            : null;

        if (link != null)
        {
            result = GetNode(link.ToNodeKey);
        }

        return result;
    }

    public bool HasLink(CommonNamedKey key, string code)
    {
        var linkKey = NodeLinkKey.Create(key, code);

        return _links.HasItem(linkKey);
    }
       
    public INodeLink GetLink(CommonNamedKey key, string code)
    {
        var linkKey = NodeLinkKey.Create(key, code);

        return _links.Get(linkKey);
    }
    #endregion
       
    public static FlowDefinition Create(            
        CommonNamedKey     initialNode,
        NodeDefinitionList nodes,
        NodeLinkList       links)

    {
        return new FlowDefinition(initialNode, nodes, links);
    }
}