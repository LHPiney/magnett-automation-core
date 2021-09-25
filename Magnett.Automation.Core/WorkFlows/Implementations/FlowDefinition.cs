using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.WorkFlows.Collections;

[assembly: InternalsVisibleTo("Magnett.Automation.Core.Test")]
namespace Magnett.Automation.Core.WorkFlows.Implementations
{
    internal class FlowDefinition : IFlowDefinition
    {
        private NodeList _nodes;
        
        public INodeBase InitialNode { get; }

        private FlowDefinition(
            INodeBase initialNode,
            NodeList nodes)
        {
            InitialNode = initialNode
                          ?? throw new ArgumentNullException(nameof(initialNode));
            _nodes = nodes 
                     ?? throw new ArgumentNullException(nameof(nodes));
        }

        public static FlowDefinition Create(            
            INodeBase initialNode,
            NodeList nodes)

        {
            return new FlowDefinition(initialNode, nodes);
        }
    }
}