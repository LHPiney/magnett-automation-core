using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Events;

public record OnNodeExecuteEvent(
    string Name, 
    string Caller, 
    CommonNamedKey NodeName) : Event(Name, Caller)
{
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