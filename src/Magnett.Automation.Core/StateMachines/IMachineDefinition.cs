using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.StateMachines
{
    public interface IMachineDefinition
    {
        IState InitialState { get; }
        bool HasState(CommonNamedKey stateKey);
        IState GetState(CommonNamedKey stateKey);
    }
}