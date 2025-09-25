using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Events;

public record OnNodeExecuteEvent(
    string Name, 
    string Caller, 
    CommonNamedKey NodeName) : Event(Name, Caller)
{
    /// <summary>
    /// Creates a new OnNodeExecuteEvent instance for the specified node.
    /// </summary>
    /// <param name="nodeName">The name of the node being executed.</param>
    /// <param name="callerName">The name of the caller method.</param>
    /// <returns>A new OnNodeExecuteEvent instance.</returns>
    public static OnNodeExecuteEvent Create(
        CommonNamedKey nodeName,
        [CallerMemberName] string callerName = "")
    {
        return new OnNodeExecuteEvent(
            nameof(OnNodeExecuteEvent),
            callerName,
            nodeName);
    }
}