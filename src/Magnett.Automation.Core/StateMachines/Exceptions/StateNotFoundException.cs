namespace Magnett.Automation.Core.StateMachines.Exceptions;

[Serializable]
public class StateNotFoundException : Exception
{
    public StateNotFoundException(string stateName): 
        base($"State [{stateName}] not found")
    {
    }
        
    protected StateNotFoundException(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
            
    }
}