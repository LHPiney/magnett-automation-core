using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Server.Events;

public record FlowDefinitionPublishedEvent(
    string Name,
    string Caller,
    string DefinitionKey,
    int Version,
    DateTime PublishedAt) : Event(Name, Caller)
{
    public static FlowDefinitionPublishedEvent Create(
        string definitionKey,
        int version,
        DateTime publishedAt,
        [CallerMemberName] string callerName = "")
    {
        return new FlowDefinitionPublishedEvent(
            nameof(FlowDefinitionPublishedEvent),
            callerName,
            definitionKey,
            version,
            publishedAt);
    }
}
