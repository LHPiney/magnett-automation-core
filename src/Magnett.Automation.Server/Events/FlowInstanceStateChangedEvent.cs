using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Server.Events;

public record FlowInstanceStateChangedEvent(
    string Name,
    string Caller,
    Guid FlowId,
    string PreviousState,
    string NewState,
    DateTime ChangedAt,
    string? Reason = null) : Event(Name, Caller)
{
    public static FlowInstanceStateChangedEvent Create(
        Guid flowId,
        string previousState,
        string newState,
        DateTime changedAt,
        string? reason = null,
        [CallerMemberName] string callerName = "")
    {
        return new FlowInstanceStateChangedEvent(
            nameof(FlowInstanceStateChangedEvent),
            callerName,
            flowId,
            previousState,
            newState,
            changedAt,
            reason);
    }
}
