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