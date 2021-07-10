namespace Magnett.Automation.Core.StateMachine
{
    public interface IMachine
    {
        IMachine Dispatch(string action);
        IState State();
    }
}