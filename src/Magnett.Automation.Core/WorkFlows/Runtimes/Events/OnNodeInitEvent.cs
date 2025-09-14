using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Events;

public record OnNodeInitEvent(
    string Name, 
    string Caller, 
    CommonNamedKey NodeName) : Event(Name, Caller)
{
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