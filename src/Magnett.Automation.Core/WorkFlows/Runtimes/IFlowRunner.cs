namespace Magnett.Automation.Core.WorkFlows.Runtimes;

public interface IFlowRunner
{
    public Context Context { get; }
    public  Task<NodeExit> Start();
}