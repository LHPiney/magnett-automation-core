using System.Threading.Tasks;

namespace Magnett.Automation.Core.WorkFlows.Implementations
{
    public abstract class NodeAsync : NodeBase, INodeAsync
    {
        protected NodeAsync(string name) : base(name)
        {
        }

        public abstract Task Execute();
    }
}