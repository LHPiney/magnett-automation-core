using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Server.Events;

public record FlowInstanceSavedEvent(
    string Name,
    string Caller,
    Guid FlowId,
    string State,
    DateTime SavedAt) : Event(Name, Caller)
{
    public static FlowInstanceSavedEvent Create(
        Guid flowId,
        string state,
        DateTime savedAt,
        [CallerMemberName] string callerName = "")
    {
        return new FlowInstanceSavedEvent(
            nameof(FlowInstanceSavedEvent),
            callerName,
            flowId,
            state,
            savedAt);
    }
}
