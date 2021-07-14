using System;
using System.Runtime.Serialization;

namespace Magnett.Automation.Core.StateMachine.Exceptions
{
    [Serializable]
    public class UndefinedStateException : Exception
    {
        public UndefinedStateException(): 
            base($"Machine without state")
        {
        }
        
        public UndefinedStateException(SerializationInfo info, StreamingContext context): 
            base(info, context)
        {
        }
    }
}