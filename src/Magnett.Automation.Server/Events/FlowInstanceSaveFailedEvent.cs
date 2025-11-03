using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Server.Events;

public record FlowInstanceSaveFailedEvent(
    string Name,
    string Caller,
    Guid FlowId,
    string ErrorMessage,
    Exception? Exception = null) : Event(Name, Caller)
{
    public static FlowInstanceSaveFailedEvent Create(
        Guid flowId,
        string errorMessage,
        Exception? exception = null,
        [CallerMemberName] string callerName = "")
    {
        return new FlowInstanceSaveFailedEvent(
            nameof(FlowInstanceSaveFailedEvent),
            callerName,
            flowId,
            errorMessage,
            exception);
    }
}
