using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Events;

public record OnNodeFailedEvent(
    string Name, 
    string Caller, 
    CommonNamedKey NodeName,
    string Code,
    string Data) : Event(Name, Caller)
{
    /// <summary>
    /// Creates a new OnNodeFailedEvent instance for the specified failed node.
    /// </summary>
    /// <param name="nodeName">The name of the failed node.</param>
    /// <param name="code">The failure code.</param>
    /// <param name="data">Additional failure data.</param>
    /// <param name="callerName">The name of the caller method.</param>
    /// <returns>A new OnNodeFailedEvent instance.</returns>
    public static OnNodeFailedEvent Create(
        CommonNamedKey nodeName,
        string code,
        string data,
        [CallerMemberName] string callerName = "")
    {
        return new OnNodeFailedEvent(
            nameof(OnNodeFailedEvent),
            callerName,
            nodeName, 
            code,
            data);
    }
}