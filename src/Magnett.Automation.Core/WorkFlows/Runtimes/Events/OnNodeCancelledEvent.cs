using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Events;

public record OnNodeCancelledEvent(
    string Name, 
    string Caller, 
    CommonNamedKey NodeName,
    string Code,
    string Data) : Event(Name, Caller)
{
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