using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Events;

public record OnNodePausedEvent(
    string Name, 
    string Caller, 
    CommonNamedKey NodeName,
    string Code,
    string Data) : Event(Name, Caller)
{
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