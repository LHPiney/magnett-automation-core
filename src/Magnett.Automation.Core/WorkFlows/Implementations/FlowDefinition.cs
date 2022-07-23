using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Collections;

[assembly: InternalsVisibleTo("Magnett.Automation.Core.UnitTest")]
namespace Magnett.Automation.Core.WorkFlows.Implementations;

internal class  FlowDefinition : IFlowDefinition
{
    private readonly NodeList     _nodes;
    private readonly NodeLinkList _links;
        
    private FlowDefinition(
        INodeBase    initialNode,
        NodeList     nodes,
        NodeLinkList links)
    {
        InitialNode = initialNode
                      ?? throw new ArgumentNullException(nameof(initialNode));
        _nodes = nodes 
                 ?? throw new ArgumentNullException(nameof(nodes));
        _links = links
                 ?? throw new ArgumentNullException(nameof(links));
    }

    #region IFlowDefinition

    public INodeBase InitialNode { get; }

    public bool HasNode(CommonNamedKey nodeKey)
    {
        return _nodes.HasItem(nodeKey);
    }
       
    public INodeBase GetNode(CommonNamedKey nodeKey)
    {
        return _nodes.Get(nodeKey);
    }

    public INodeBase GetNode(INodeBase sourceNode, string code)
    {
        INodeBase result = null;

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
        INodeBase    initialNode,
        NodeList     nodes,
        NodeLinkList links)

    {
        return new FlowDefinition(initialNode, nodes, links);
    }
}