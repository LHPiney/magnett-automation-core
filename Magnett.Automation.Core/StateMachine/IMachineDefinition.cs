namespace Magnett.Automation.Core.StateMachine
{
    public interface IMachineDefinition
    {
        IState InitialState { get; }
        
        bool HasState(string stateName);
        IState GetState(string stateName);
    }
}