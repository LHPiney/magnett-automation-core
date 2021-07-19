using System;
using System.Runtime.Serialization;

namespace Magnett.Automation.Core.StateMachine.Exceptions
{
    public class InvalidMachineDefinitionException : Exception
    {
        public InvalidMachineDefinitionException(string message)
            : base($"Invalid Machine definition [{message}]")
        {
        }
    }
}