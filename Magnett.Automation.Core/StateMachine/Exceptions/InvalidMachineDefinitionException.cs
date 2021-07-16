using System;
using System.Runtime.Serialization;

namespace Magnett.Automation.Core.StateMachine.Exceptions
{
    [Serializable]
    public class InvalidMachineDefinitionException : Exception
    {
        public InvalidMachineDefinitionException(string message)
            : base($"Invalid Machine definition [{message}]")
        {
        }
        
        public InvalidMachineDefinitionException(SerializationInfo info, StreamingContext context): 
            base(info, context)
        {
        }
    }
}