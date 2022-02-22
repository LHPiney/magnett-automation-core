using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;

namespace Magnett.Automation.Core.WorkFlows.Implementations
{
    public abstract class NodeAsync : NodeBase, INodeAsync
    {
        protected NodeAsync(string name) : base(name)
        {
            
        }
        
        protected NodeAsync(CommonNamedKey key) : base(key)
        {
            
        }

        public abstract Task<NodeExit> Execute(Context context);
    }
}