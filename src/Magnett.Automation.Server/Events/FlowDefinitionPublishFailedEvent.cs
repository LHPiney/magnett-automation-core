using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Server.Events;

public record FlowDefinitionPublishFailedEvent(
    string Name,
    string Caller,
    string DefinitionKey,
    int? Version,
    string ErrorMessage,
    Exception? Exception = null) : Event(Name, Caller)
{
    public static FlowDefinitionPublishFailedEvent Create(
        string definitionKey,
        int? version,
        string errorMessage,
        Exception? exception = null,
        [CallerMemberName] string callerName = "")
    {
        return new FlowDefinitionPublishFailedEvent(
            nameof(FlowDefinitionPublishFailedEvent),
            callerName,
            definitionKey,
            version,
            errorMessage,
            exception);
    }
}
