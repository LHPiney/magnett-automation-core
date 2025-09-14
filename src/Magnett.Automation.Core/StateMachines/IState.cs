namespace Magnett.Automation.Core.StateMachines;

/// <summary>
/// Represents a generic state in a state machine. The state holds information
/// identifying itself and provides methods to manage transitions triggered by
/// specific actions. The state is also capable of determining whether it is
/// considered a final state in the machine's lifecycle.
/// </summary>
public interface IState
{
    public CommonNamedKey Key { get; }
    public ITransition ManageAction(CommonNamedKey actionName);
    public IEnumerable<CommonNamedKey> TargetStates();
    public IEnumerable<CommonNamedKey> AvaliableActions();
    public bool IsFinalState();

    public bool Equals(CommonNamedKey obj)
    {
        return obj != null && Key.Equals(obj);
    }
}