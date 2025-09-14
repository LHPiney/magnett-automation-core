using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.StateMachines.Events;

public record OnMachineInit(
    string Name, 
    string Caller, 
    Guid MachineId) : Event(Name, Caller)
{
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