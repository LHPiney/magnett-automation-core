using System;

namespace Magnett.Automation.Core.StateMachine
{
    internal class Transition : ITransition
    {
        public string ToStateName { get; private init; }
        public string Action { get; private init; } 
        
        private Transition(
            string action,
            string targetStateName)
        {
            if (string.IsNullOrEmpty(action)) 
                throw new ArgumentNullException(nameof(action));
            
            if (string.IsNullOrEmpty(targetStateName)) 
                throw new ArgumentNullException(nameof(targetStateName));
            
            Action = action;
            ToStateName = targetStateName;
        }

        public static Transition Create(
            string action,
            string targetStateName)
        {
            return new(action, targetStateName);
        }
        
    }
}