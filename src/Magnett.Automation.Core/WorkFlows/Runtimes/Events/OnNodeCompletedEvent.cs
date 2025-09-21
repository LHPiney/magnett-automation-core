using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Events;

public record OnNodeCompletedEvent(
    string Name, 
    string Caller, 
    CommonNamedKey NodeName,
    string Code,
    string Data) : Event(Name, Caller)
{
    /// <summary>
    /// Creates a new OnNodeCompletedEvent instance for the specified completed node.
    /// </summary>
    /// <param name="nodeName">The name of the completed node.</param>
    /// <param name="code">The completion code.</param>
    /// <param name="data">Additional completion data.</param>
    /// <param name="callerName">The name of the caller method.</param>
    /// <returns>A new OnNodeCompletedEvent instance.</returns>
    public static OnNodeCompletedEvent Create(
        CommonNamedKey nodeName,
        string code,
        string data,
        [CallerMemberName] string callerName = "")
    {
        return new OnNodeCompletedEvent(
            nameof(OnNodeCompletedEvent),
            callerName,
            nodeName,
            code,
            data);
    }
}