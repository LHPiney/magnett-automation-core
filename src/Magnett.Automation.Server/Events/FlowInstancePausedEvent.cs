using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Server.Events;

public record FlowInstancePausedEvent(
    string Name,
    string Caller,
    Guid FlowId,
    DateTime PausedAt,
    string? Reason = null) : Event(Name, Caller)
{
    public static FlowInstancePausedEvent Create(
        Guid flowId,
        DateTime pausedAt,
        string? reason = null,
        [CallerMemberName] string callerName = "")
    {
        return new FlowInstancePausedEvent(
            nameof(FlowInstancePausedEvent),
            callerName,
            flowId,
            pausedAt,
            reason);
    }
}
