using Magnett.Automation.Core.WorkFlows.Definitions.Collections;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Collections;
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

    public INodeDefinition GetNode(INodeBase sourceNode, string code)
    {
        INodeDefinition result = null;

        var link = HasLink(sourceNode, code)
            ? GetLink(sourceNode, code)
            : null;

        if (link != null)
        {
            result = GetNode(link.ToNodeKey);
        }

        return result;
    }

    public bool HasLink(INodeBase sourceNode, string code)
    {
        var linkKey = NodeLinkKey.Create(sourceNode?.Key, code);

        return _links.HasItem(linkKey);
    }
       
    public INodeLink GetLink(INodeBase sourceNode, string code)
    {
        var linkKey = NodeLinkKey.Create(sourceNode?.Key, code);

        return  _links.Get(linkKey);
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