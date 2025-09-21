using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.StateMachines.Events;

public record OnMachineChangeState(
    string Name, 
    string Caller,
    Guid MachineId,
    CommonNamedKey SourceState,
    CommonNamedKey TargetState,
    CommonNamedKey Action) : Event(Name, Caller)
{
    /// <summary>
    /// Creates a new OnMachineChangeState event instance for the specified machine state change.
    /// </summary>
    /// <param name="MachineId">The unique identifier of the machine.</param>
    /// <param name="SourceState">The source state key.</param>
    /// <param name="TargetState">The target state key.</param>
    /// <param name="Action">The action that triggered the state change.</param>
    /// <param name="callerName">The name of the caller method.</param>
    /// <returns>A new OnMachineChangeState event instance.</returns>
    public static OnMachineChangeState Create(
        Guid MachineId,
        CommonNamedKey SourceState,
        CommonNamedKey TargetState,
        CommonNamedKey Action,
        [CallerMemberName] string callerName = "")
    {
        return new OnMachineChangeState(
            nameof(OnMachineChangeState),
            callerName,
            MachineId,
            SourceState,
            TargetState,
            Action);
    }
}