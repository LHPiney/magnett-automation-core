namespace Magnett.Automation.Core.StateMachines
{
    public interface IMachine
    {
        IState State { get; }
        
        IMachine Dispatch(string actionName);
    }
}