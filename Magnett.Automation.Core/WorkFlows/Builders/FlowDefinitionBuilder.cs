using System;
using Magnett.Automation.Core.WorkFlows.Collections;
using Magnett.Automation.Core.WorkFlows.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Builders
{
    public class FlowDefinitionBuilder
    {
        private readonly NodeList _nodeList;
        private INodeBase _initialNode;

        private FlowDefinitionBuilder()
        {
            _nodeList = new NodeList();
        }

        public FlowDefinitionBuilder WithInitialNode(INodeBase node)
        {
            _nodeList.Add(node.Key, node);

            _initialNode = node;

            return this;
        }
        
        public FlowDefinitionBuilder WithNode(INodeBase node)
        {
            _nodeList.Add(node.Key, node);

            return this;
        }

        public IFlowDefinition Build()
        {
            return FlowDefinition.Create(_initialNode, _nodeList);
        }
    }
}