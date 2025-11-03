using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Server.Events;

public record FlowInstanceResumedEvent(
    string Name,
    string Caller,
    Guid FlowId,
    DateTime ResumedAt) : Event(Name, Caller)
{
    public static FlowInstanceResumedEvent Create(
        Guid flowId,
        DateTime resumedAt,
        [CallerMemberName] string callerName = "")
    {
        return new FlowInstanceResumedEvent(
            nameof(FlowInstanceResumedEvent),
            callerName,
            flowId,
            resumedAt);
    }
}
