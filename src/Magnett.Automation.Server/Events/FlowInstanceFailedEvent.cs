using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Server.Events;

public record FlowInstanceFailedEvent(
    string Name,
    string Caller,
    Guid FlowId,
    string ErrorMessage,
    Exception? Exception = null,
    DateTime FailedAt = default) : Event(Name, Caller)
{
    public static FlowInstanceFailedEvent Create(
        Guid flowId,
        string errorMessage,
        Exception? exception = null,
        DateTime? failedAt = null,
        [CallerMemberName] string callerName = "")
    {
        return new FlowInstanceFailedEvent(
            nameof(FlowInstanceFailedEvent),
            callerName,
            flowId,
            errorMessage,
            exception,
            failedAt ?? DateTime.UtcNow);
    }
}
