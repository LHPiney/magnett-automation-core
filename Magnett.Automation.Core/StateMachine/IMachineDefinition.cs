using Magnett.Automation.Core.Common;

namespace Magnett.Automation.Core.StateMachine
{
    public interface IMachineDefinition
    {
        IState InitialState { get; }
        bool HasState(CommonNamedKey stateKey);
        IState GetState(CommonNamedKey stateKey);
    }
}