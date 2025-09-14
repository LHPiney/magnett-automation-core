namespace Magnett.Automation.Core.StateMachines.Exceptions;

[Serializable]
public class InvalidMachineDefinitionException : Exception
{
    public InvalidMachineDefinitionException(string message)
        : base($"Invalid Machine definition [{message}]")
    {
    }
        
    [Obsolete("Obsolete")]
    protected InvalidMachineDefinitionException(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
            
    }
}