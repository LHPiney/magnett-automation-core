using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.StateMachines
{
    public interface ITransition
    {
        public CommonNamedKey ToStateKey { get; }
        public CommonNamedKey ActionKey { get; } 
    }
}