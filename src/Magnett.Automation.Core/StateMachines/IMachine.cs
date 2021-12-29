using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.StateMachines
{
    public interface IMachine
    {
        IState State { get; }

        IMachine Dispatch(Enumeration action);
        IMachine Dispatch(string actionName);
    }
}