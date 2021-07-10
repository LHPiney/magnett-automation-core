using System;

namespace Magnett.Automation.Core.StateMachine.Exceptions
{
    [Serializable]
    public class ActionNotFoundException : Exception 
    {
        public ActionNotFoundException(string stateName, string actionName)
            : base($"Action [{actionName}] not found in State [{stateName}]")
        {

        }
    }
}