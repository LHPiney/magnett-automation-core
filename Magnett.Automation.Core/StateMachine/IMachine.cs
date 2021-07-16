namespace Magnett.Automation.Core.StateMachine
{
    public interface IMachine
    {
        IState State { get; }
        
        IMachine Dispatch(string action);

    }
}