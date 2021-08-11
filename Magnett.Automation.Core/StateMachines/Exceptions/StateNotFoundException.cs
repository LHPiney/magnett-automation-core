using System;

namespace Magnett.Automation.Core.StateMachines.Exceptions
{
    public class StateNotFoundException : Exception
    {
        public StateNotFoundException(string stateName): 
            base($"State [{stateName}] not found")
        {
        }
    }
}