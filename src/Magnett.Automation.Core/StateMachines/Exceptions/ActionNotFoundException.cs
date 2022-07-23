namespace Magnett.Automation.Core.StateMachines.Exceptions;

[Serializable]
public class ActionNotFoundException : Exception 
{
    public ActionNotFoundException(string stateName, string actionName) : 
        base($"Action [{actionName}] not found in State [{stateName}]")
    {

    }

    protected ActionNotFoundException(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
            
    }
}