namespace Magnett.Automation.Core.StateMachines;

/// <summary>
/// Represents a state machine interface that defines the core functionalities
/// for managing state transitions and maintaining the current state.
/// Provides methods to dispatch actions, restart the machine, and determine equality with other objects.
/// Library provides a default implementation Machine <seealso cref="Machine"/> 
/// </summary>
public interface IMachine
{
    public Guid Id { get; }
    public IState Current { get; }
    public Task<IMachine> DispatchAsync(Enumeration action);
    public Task<IMachine> DispatchAsync(string actionName);
    public IMachineDefinition GetDefinition();
    public Task<IMachine> ReStartAsync();
    public bool Equals(CommonNamedKey obj);
    public bool Equals(Enumeration obj);
}