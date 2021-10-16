using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.WorkFlows.Implementations
{
    public abstract class Node : NodeBase, INode
    {
        protected Node(string name) : base(name)
        {
            
        }
        
        protected Node(CommonNamedKey key) : base(key)
        {
            
        }
        
        public abstract void Execute();
    }
}