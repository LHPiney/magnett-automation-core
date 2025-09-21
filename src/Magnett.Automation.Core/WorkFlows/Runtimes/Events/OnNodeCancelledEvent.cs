using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Events;

public record OnNodeCancelledEvent(
    string Name, 
    string Caller, 
    CommonNamedKey NodeName,
    string Code,
    string Data) : Event(Name, Caller)
{
    /// <summary>
    /// Creates a new OnNodeCancelledEvent instance for the specified cancelled node.
    /// </summary>
    /// <param name="nodeName">The name of the cancelled node.</param>
    /// <param name="code">The cancellation code.</param>
    /// <param name="data">Additional cancellation data.</param>
    /// <param name="callerName">The name of the caller method.</param>
    /// <returns>A new OnNodeCancelledEvent instance.</returns>
    public static OnNodeCancelledEvent Create(
        CommonNamedKey nodeName,
        string code,
        string data,
        [CallerMemberName] string callerName = "")
    {
        return new OnNodeCancelledEvent(
            nameof(OnNodeCancelledEvent),
            callerName,
            nodeName,
            code,
            data);
    }
}