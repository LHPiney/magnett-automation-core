using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Commons;

[assembly: InternalsVisibleTo("Magnett.Automation.Core.UnitTest")]
namespace Magnett.Automation.Core.StateMachines
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