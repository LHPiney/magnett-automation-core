using System;
using System.Runtime.Serialization;

namespace Magnett.Automation.Core.StateMachine.Exceptions
{
    public class ActionNotFoundException : Exception 
    {
        public ActionNotFoundException(string stateName, string actionName)
            : base($"Action [{actionName}] not found in State [{stateName}]")
        {

        }
    }
}