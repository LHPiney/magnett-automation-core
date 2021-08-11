using System;

namespace Magnett.Automation.Core.StateMachines.Exceptions
{
    public class InvalidMachineDefinitionException : Exception
    {
        public InvalidMachineDefinitionException(string message)
            : base($"Invalid Machine definition [{message}]")
        {
        }
    }
}