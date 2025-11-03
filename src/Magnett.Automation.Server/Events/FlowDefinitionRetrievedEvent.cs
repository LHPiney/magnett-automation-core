using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Server.Events;

public record FlowDefinitionRetrievedEvent(
    string Name,
    string Caller,
    string DefinitionKey,
    int? Version,
    DateTime RetrievedAt) : Event(Name, Caller)
{
    public static FlowDefinitionRetrievedEvent Create(
        string definitionKey,
        int? version,
        DateTime retrievedAt,
        [CallerMemberName] string callerName = "")
    {
        return new FlowDefinitionRetrievedEvent(
            nameof(FlowDefinitionRetrievedEvent),
            callerName,
            definitionKey,
            version,
            retrievedAt);
    }
}
