using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Server.Events;

public record FlowInstanceRetrievedEvent(
    string Name,
    string Caller,
    Guid FlowId,
    DateTime RetrievedAt) : Event(Name, Caller)
{
    public static FlowInstanceRetrievedEvent Create(
        Guid flowId,
        DateTime retrievedAt,
        [CallerMemberName] string callerName = "")
    {
        return new FlowInstanceRetrievedEvent(
            nameof(FlowInstanceRetrievedEvent),
            callerName,
            flowId,
            retrievedAt);
    }
}
