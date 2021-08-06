using System.Runtime.CompilerServices;

using Magnett.Automation.Core.Common;

[assembly: InternalsVisibleTo("Magnett.Automation.Core.Test")]
namespace Magnett.Automation.Core.StateMachine
{
    internal class Transition : ITransition
    {
        public CommonNamedKey ToStateKey { get; }
        public CommonNamedKey ActionKey { get; } 
        
        private Transition(
            string actionName,
            string toStateName)
        {
            ActionKey  = CommonNamedKey.Create(actionName);
            ToStateKey = CommonNamedKey.Create(toStateName);
        }

        public static Transition Create(
            string actionName,
            string toStateName)
        {
            return new(actionName, toStateName);
        }
        
    }
}