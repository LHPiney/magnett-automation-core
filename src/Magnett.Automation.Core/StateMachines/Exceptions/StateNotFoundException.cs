using System;
using System.Runtime.Serialization;

namespace Magnett.Automation.Core.StateMachines.Exceptions
{
    [Serializable]
    public class StateNotFoundException : Exception
    {
        public StateNotFoundException(string stateName): 
            base($"State [{stateName}] not found")
        {
        }
        
        public StateNotFoundException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            
        }
    }
}