using Magnett.Automation.Core.Contexts;

namespace Magnett.Automation.Core.WorkFlows
{
    public interface INode : INodeBase
    {
        public NodeExit Execute(Context context);
    }
}