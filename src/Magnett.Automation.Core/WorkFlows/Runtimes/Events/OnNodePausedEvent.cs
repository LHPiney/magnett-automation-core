using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Events;

public record OnNodePausedEvent(
    string Name, 
    string Caller, 
    CommonNamedKey NodeName,
    string Code,
    string Data) : Event(Name, Caller)
{
    /// <summary>
    /// Creates a new OnNodePausedEvent instance for the specified paused node.
    /// </summary>
    /// <param name="nodeName">The name of the paused node.</param>
    /// <param name="code">The pause code.</param>
    /// <param name="data">Additional pause data.</param>
    /// <param name="callerName">The name of the caller method.</param>
    /// <returns>A new OnNodePausedEvent instance.</returns>
    public static OnNodePausedEvent Create(
        CommonNamedKey nodeName,
        string code,
        string data,
        [CallerMemberName] string callerName = "")
    {
        return new OnNodePausedEvent(
            nameof(OnNodePausedEvent),
            callerName,
            nodeName,
            code,
            data);
    }
}