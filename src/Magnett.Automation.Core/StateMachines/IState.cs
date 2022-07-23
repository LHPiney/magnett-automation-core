namespace Magnett.Automation.Core.StateMachines;

public interface IState
{
    CommonNamedKey Key { get; }

    public ITransition ManageAction(CommonNamedKey actionName);

    public bool IsFinalState();
}