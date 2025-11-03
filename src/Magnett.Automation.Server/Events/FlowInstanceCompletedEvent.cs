using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Server.Events;

public record FlowInstanceCompletedEvent(
    string Name,
    string Caller,
    Guid FlowId,
    DateTime CompletedAt,
    bool Success,
    string? FailureReason = null) : Event(Name, Caller)
{
    public static FlowInstanceCompletedEvent Create(
        Guid flowId,
        DateTime completedAt,
        bool success,
        string? failureReason = null,
        [CallerMemberName] string callerName = "")
    {
        return new FlowInstanceCompletedEvent(
            nameof(FlowInstanceCompletedEvent),
            callerName,
            flowId,
            completedAt,
            success,
            failureReason);
    }
}
