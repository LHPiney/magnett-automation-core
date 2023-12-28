namespace Magnett.Automation.Core.WorkFlows.Runtimes;

public interface INodeAsync : INodeBase
{
    Task<NodeExit> Execute(Context context);
}