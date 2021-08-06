using Magnett.Automation.Core.Common;

namespace Magnett.Automation.Core.StateMachine
{
    public interface IState
    {
        CommonNamedKey Key { get; }

        public ITransition ManageAction(CommonNamedKey actionName);

        public bool IsFinalState();
    }
}