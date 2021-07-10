using System;

namespace Magnett.Automation.Core.StateMachine.Exceptions
{
    [Serializable]
    public class UndefinedStateException : Exception
    {
        public UndefinedStateException()
            : base($"Machine without state")
        {

        }
    }
}