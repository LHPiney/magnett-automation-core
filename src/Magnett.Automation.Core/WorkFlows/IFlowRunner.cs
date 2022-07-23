namespace Magnett.Automation.Core.WorkFlows;

public interface IFlowRunner
{
    public Context FlowContext { get; }
    public  Task<NodeExit> Start();
}