using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Server.Events;

public record FlowInstanceCreatedEvent(
    string Name,
    string Caller,
    Guid FlowId,
    string DefinitionKey,
    int DefinitionVersion,
    DateTime CreatedAt) : Event(Name, Caller)
{
    public static FlowInstanceCreatedEvent Create(
        Guid flowId,
        string definitionKey,
        int definitionVersion,
        DateTime createdAt,
        [CallerMemberName] string callerName = "")
    {
        return new FlowInstanceCreatedEvent(
            nameof(FlowInstanceCreatedEvent),
            callerName,
            flowId,
            definitionKey,
            definitionVersion,
            createdAt);
    }
}
