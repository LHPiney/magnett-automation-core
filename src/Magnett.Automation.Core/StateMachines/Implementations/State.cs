﻿[assembly: InternalsVisibleTo("Magnett.Automation.Core.UnitTest")]
namespace Magnett.Automation.Core.StateMachines.Implementations;

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
        return new(name, transitionList);
    }
}