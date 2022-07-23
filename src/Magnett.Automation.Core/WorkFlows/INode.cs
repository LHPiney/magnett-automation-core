namespace Magnett.Automation.Core.WorkFlows;

public interface INode : INodeBase
{
    public NodeExit Execute(Context context);
}