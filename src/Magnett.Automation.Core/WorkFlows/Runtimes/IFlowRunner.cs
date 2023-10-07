namespace Magnett.Automation.Core.WorkFlows.Runtimes;

public interface IFlowRunner
{
    public Context FlowContext { get; }
    public  Task<NodeExit> Start();
}