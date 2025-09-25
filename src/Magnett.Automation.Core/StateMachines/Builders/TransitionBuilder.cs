namespace Magnett.Automation.Core.StateMachines.Builders;

public class TransitionBuilder
{
    private readonly string _actionName;
    private readonly Func<ITransition, StateBuilder> _storeAction;
    private Func<ITransition, Task>? _asyncAction;

    private TransitionBuilder(
        string actionName,
        Func<ITransition, StateBuilder> storeAction)
    {
        if (string.IsNullOrEmpty(actionName))
            throw new ArgumentNullException(nameof(actionName));

        _actionName = actionName;
        _storeAction = storeAction ?? throw new ArgumentNullException(nameof(storeAction));
    }

    public TransitionBuilder OnCallBackAsync(Func<ITransition, Task> asyncAction)
    {
        _asyncAction = asyncAction;
        return this;
    }

    public StateBuilder ToState(Enumeration state)
    {
        return ToState(state.Name);
    }

    public StateBuilder ToState(string stateName)
    {
        return _storeAction.Invoke(
            Transition.Create(_actionName, stateName, _asyncAction));
    }

    /// <summary>
    /// Creates a new TransitionBuilder instance for the specified action.
    /// </summary>
    /// <param name="actionName">The name of the transition action.</param>
    /// <param name="storeAction">Function to store the transition configuration.</param>
    /// <returns>A new TransitionBuilder instance.</returns>
    public static TransitionBuilder Create(
        string actionName,
        Func<ITransition, StateBuilder> storeAction)
    {
        return new TransitionBuilder(actionName, storeAction);
    }
}