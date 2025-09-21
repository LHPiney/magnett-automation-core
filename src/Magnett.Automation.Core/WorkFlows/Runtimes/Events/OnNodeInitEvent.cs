using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Events;

public record OnNodeInitEvent(
    string Name, 
    string Caller, 
    CommonNamedKey NodeName) : Event(Name, Caller)
{
    /// <summary>
    /// Creates a new OnNodeInitEvent instance for the specified initialized node.
    /// </summary>
    /// <param name="nodeName">The name of the initialized node.</param>
    /// <param name="callerName">The name of the caller method.</param>
    /// <returns>A new OnNodeInitEvent instance.</returns>
    public static OnNodeInitEvent Create(
        CommonNamedKey nodeName,
        [CallerMemberName] string callerName = "")
    {
        return new OnNodeInitEvent(
            nameof(OnNodeInitEvent),
            callerName,
            nodeName);
    }
}