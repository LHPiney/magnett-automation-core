namespace Magnett.Automation.Core.WorkFlows.Implementations
{
    public abstract class Node : NodeBase, INode
    {
        protected Node(string name) : base(name)
        {
        }
        
        public abstract void Execute();
    }
}