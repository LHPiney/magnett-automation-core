namespace Magnett.Automation.Core.WorkFlows.Runtimes;

public interface INode : INodeBase
{
    public NodeExit Execute(Context context);
}