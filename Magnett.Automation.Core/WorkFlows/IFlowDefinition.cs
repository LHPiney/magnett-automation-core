using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.WorkFlows
{
    public interface IFlowDefinition
    {
        public INodeBase InitialNode { get; }
        public bool HasLink(INodeBase sourceNode, string code);        
        public INodeLink GetLink(INodeBase sourceNode, string code);
        public bool HasNode(CommonNamedKey nodeKey);
        public INodeBase GetNode(CommonNamedKey nodeKey);
        public INodeBase GetNode(INodeBase sourceNode, string code);
    }
}