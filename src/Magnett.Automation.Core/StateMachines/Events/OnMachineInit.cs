using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.StateMachines.Events;

public record OnMachineInit(
    string Name, 
    string Caller, 
    Guid MachineId) : Event(Name, Caller)
{
    /// <summary>
    /// Creates a new OnMachineInit event instance for the specified machine.
    /// </summary>
    /// <param name="machineId">The unique identifier of the machine.</param>
    /// <param name="callerName">The name of the caller method.</param>
    /// <returns>A new OnMachineInit event instance.</returns>
    public static OnMachineInit Create(
        Guid machineId,
        [CallerMemberName] string callerName = "")
    {
        return new OnMachineInit(
            nameof(OnMachineChangeState),
            callerName,
            machineId);
    }
}