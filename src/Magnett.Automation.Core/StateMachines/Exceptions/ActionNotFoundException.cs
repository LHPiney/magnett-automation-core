using System;
using System.Runtime.Serialization;

namespace Magnett.Automation.Core.StateMachines.Exceptions
{
    [Serializable]
    public class ActionNotFoundException : Exception 
    {
        public ActionNotFoundException(string stateName, string actionName) : 
            base($"Action [{actionName}] not found in State [{stateName}]")
        {

        }

        public ActionNotFoundException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            
        }
    }
}