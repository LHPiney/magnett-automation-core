namespace Magnett.Automation.Core.StateMachines;

public interface IMachine
{
    IState State { get; }

    IMachine Dispatch(Enumeration action);
    IMachine Dispatch(string actionName);
    public bool Equals(CommonNamedKey obj);
    public bool Equals(Enumeration obj);
}