namespace Magnett.Automation.Core.StateMachines.Exceptions;

[Serializable]
public class StateNotFoundException : Exception
{
    public StateNotFoundException(string stateName): 
        base($"Current [{stateName}] not found")
    {
    }
        
    [Obsolete("Obsolete")]
    protected StateNotFoundException(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
            
    }
}