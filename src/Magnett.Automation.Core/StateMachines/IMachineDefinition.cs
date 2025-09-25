namespace Magnett.Automation.Core.StateMachines;

/// <summary>
/// Defines the interface for state machine definitions.
/// Provides methods to manage states and retrieve the initial state.
/// The interface allows checking for the existence of states, 
/// </summary>
public interface IMachineDefinition
{
    public IState InitialState { get; }
    public bool HasState(CommonNamedKey stateKey);
    public IState GetState(CommonNamedKey stateKey);
}