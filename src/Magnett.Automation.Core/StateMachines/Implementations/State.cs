using System.Linq;

[assembly: InternalsVisibleTo("Magnett.Automation.Core.UnitTest")]
namespace Magnett.Automation.Core.StateMachines.Implementations;

/// <summary>
/// Represents a state in a state machine. A state encapsulates a
/// collection of transitions and provides methods to manage these transitions
/// based on actions associated with the state. It also provides information
/// about itself, such as whether it is a final state and its unique identifier.
/// </summary>
internal class State : IState
{
    private readonly TransitionList _transitionList;
        
    public CommonNamedKey Key { get; }

    private State(string name, TransitionList transitionList)
    {
        Key = CommonNamedKey.Create(name);

        _transitionList = transitionList
                          ?? throw new ArgumentNullException(nameof(transitionList));
    }
        
    private bool CanManageAction(CommonNamedKey actionName)
    {
        return _transitionList
            .HasItem(actionName);
    }

    #region IState
    public ITransition ManageAction(CommonNamedKey actionName)
    {
        return CanManageAction(actionName)
            ? _transitionList.Get(actionName)
            : throw new ActionNotFoundException(Key.Name, actionName.Name);
    }

    public IEnumerable<CommonNamedKey> TargetStates()
    {
        return _transitionList.GetValues()
            .Select(t => t.ToStateKey);

    }

    public IEnumerable<CommonNamedKey> AvaliableActions()
    {
        return _transitionList.GetValues()
            .Select(t => t.ActionKey);
    }
    
    public bool IsFinalState()
    {
        return _transitionList.IsEmpty();
    }

    #endregion

    public override string ToString()
    {
        return Key.Name;
    }

    public static State Create(string name, TransitionList transitionList)
    {
        return new State(name, transitionList);
    }
}