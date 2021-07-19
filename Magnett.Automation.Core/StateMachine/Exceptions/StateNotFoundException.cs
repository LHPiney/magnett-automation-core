using System;
using System.Runtime.Serialization;

namespace Magnett.Automation.Core.StateMachine.Exceptions
{
    public class StateNotFoundException : Exception
    {
        public StateNotFoundException(string stateName): 
            base($"State [{stateName}] not found")
        {
        }
    }
}