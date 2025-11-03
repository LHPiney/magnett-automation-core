using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Server.Events;

public record FlowInstanceCreationFailedEvent(
    string Name,
    string Caller,
    string DefinitionKey,
    string ErrorMessage,
    Exception? Exception = null) : Event(Name, Caller)
{
    public static FlowInstanceCreationFailedEvent Create(
        string definitionKey,
        string errorMessage,
        Exception? exception = null,
        [CallerMemberName] string callerName = "")
    {
        return new FlowInstanceCreationFailedEvent(
            nameof(FlowInstanceCreationFailedEvent),
            callerName,
            definitionKey,
            errorMessage,
            exception);
    }
}
