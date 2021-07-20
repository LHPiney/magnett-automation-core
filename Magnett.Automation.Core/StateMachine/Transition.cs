using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Magnett.Automation.Core.Test")]
namespace Magnett.Automation.Core.StateMachine
{
    internal class Transition : ITransition
    {
        public string ToStateName { get; }
        public string Action { get; } 
        
        private Transition(
            string action,
            string targetStateName)
        {
            if (string.IsNullOrEmpty(action)) 
                throw new ArgumentNullException(nameof(action));
            
            if (string.IsNullOrEmpty(targetStateName)) 
                throw new ArgumentNullException(nameof(targetStateName));
            
            Action      = action;
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