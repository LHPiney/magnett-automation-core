using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;

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
        
        public abstract NodeExit Execute(Context context);
    }
}