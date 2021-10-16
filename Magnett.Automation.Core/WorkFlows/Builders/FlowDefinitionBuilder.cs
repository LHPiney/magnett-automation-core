using Magnett.Automation.Core.StateMachines.Builders;
using Magnett.Automation.Core.WorkFlows.Collections;
using Magnett.Automation.Core.WorkFlows.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Builders
{
    public class FlowDefinitionBuilder
    {
        private readonly NodeList     _nodes;
        private readonly NodeLinkList _links;
        
        private INodeBase _initialNode;

        private FlowDefinitionBuilder()
        {
            _nodes = new NodeList();
            _links = new NodeLinkList();
        }

        private INodeLinkBuilder StoreNodeLink(
            INodeBase sourceNode,
            INodeLink nodeLink)
        {
            _links.Add(nodeLink.Key, nodeLink);

            return NodeLinkBuilder.Create(
                sourceNode, 
                StoreNodeLink, 
                () => this);
        }

        public INodeLinkBuilder WithInitialNode(INodeBase node)
        {
            _initialNode = node;

            return WithNode(node);
        }

        public INodeLinkBuilder WithNode(INodeBase node)
        {
            _nodes.Add(node.Key, node);

            return NodeLinkBuilder.Create(
                node, 
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
}