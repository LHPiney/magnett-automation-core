namespace Magnett.Automation.Core.StateMachines.Builders;

public class StateBuilder
{
    private readonly TransitionList _transitions;
    private readonly Func<IState, bool, MachineDefinitionBuilder> _storeAction;
        
    public string StateName { get; }

    public bool IsInitialState { get; }

    private StateBuilder(
        string stateName,
        bool isInitialState,
        Func<IState, bool, MachineDefinitionBuilder> storeAction)
    {
        if (string.IsNullOrEmpty(stateName))
            throw new ArgumentNullException(nameof(stateName));
                    
        StateName = stateName;
        IsInitialState = isInitialState;
        _storeAction = storeAction;
        _transitions = TransitionList.Create();
    }

    private StateBuilder StoreTransition(ITransition transition)
    {
        _transitions.Add(transition.ActionKey, transition);

        return this;
    }
        
    public TransitionBuilder OnAction(Enumeration action)
    {
        return TransitionBuilder.Create(action.Name, StoreTransition);
    }        
        
    public TransitionBuilder OnAction(string actionName)
    {
        return TransitionBuilder.Create(actionName, StoreTransition);
    }

    public MachineDefinitionBuilder Build()
    {
        var state = State.Create(StateName, _transitions);

        return _storeAction.Invoke(state, IsInitialState);
    }

    public static StateBuilder Create(
        string stateName,
        bool isInitialState,
        Func<IState, bool, MachineDefinitionBuilder> storeAction)
    {
        return new StateBuilder(stateName, isInitialState, storeAction);
    }
}