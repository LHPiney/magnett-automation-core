using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.WorkFlows.Collections;

[assembly: InternalsVisibleTo("Magnett.Automation.Core.UnitTest")]
namespace Magnett.Automation.Core.WorkFlows.Implementations
{
    internal class FlowDefinition : IFlowDefinition
    {
        private readonly NodeList _nodes;
        private readonly NodeLinkList _links;
        
        public INodeBase InitialNode { get; }

        private FlowDefinition(
            INodeBase initialNode,
            NodeList nodes,
            NodeLinkList links)
        {
            InitialNode = initialNode
                          ?? throw new ArgumentNullException(nameof(initialNode));
            _nodes = nodes 
                     ?? throw new ArgumentNullException(nameof(nodes));
            _links = links
                     ?? throw new ArgumentNullException(nameof(links));
        }

        public static FlowDefinition Create(            
            INodeBase initialNode,
            NodeList nodes,
            NodeLinkList links)

        {
            return new FlowDefinition(initialNode, nodes, links);
        }
    }
}