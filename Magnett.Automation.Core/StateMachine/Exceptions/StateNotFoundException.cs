using System;

namespace Magnett.Automation.Core.StateMachine.Exceptions
{
    [Serializable]
    public class StateNotFoundException : Exception
    {
        public StateNotFoundException(string stateName)
            : base($"State [{stateName}] not found")
        {

        }
    }
}