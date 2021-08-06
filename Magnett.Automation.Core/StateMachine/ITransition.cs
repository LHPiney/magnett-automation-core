using Magnett.Automation.Core.Common;

namespace Magnett.Automation.Core.StateMachine
{
    public interface ITransition
    {
        public CommonNamedKey ToStateKey { get; }
        public CommonNamedKey ActionKey { get; } 
    }
}