[assembly: InternalsVisibleTo("Magnett.Automation.Core.UnitTest")]
namespace Magnett.Automation.Core.StateMachines.Implementations;

/// <summary>
/// Represents a transition in a state machine, defining a specific action and the target state it leads to.
/// </summary>
internal class Transition : ITransition
{
    private readonly Func<ITransition, Task> onTransitionAsync = null!;

    public CommonNamedKey ToStateKey { get; }
    public CommonNamedKey ActionKey { get; }
    public Func<ITransition, Task>? OnTransitionAsync => onTransitionAsync;


    private Transition(
        string actionName,
        string toStateName, 
        Func<ITransition, Task>? onTransitionAsync = null)
    {
        ActionKey  = CommonNamedKey.Create(actionName);
        ToStateKey = CommonNamedKey.Create(toStateName);
        if (onTransitionAsync != null)
            this.onTransitionAsync = onTransitionAsync;
    }

    /// <summary>
    /// Creates a new Transition instance with the specified action and target state.
    /// </summary>
    /// <param name="actionName">The name of the transition action.</param>
    /// <param name="toStateName">The name of the target state.</param>
    /// <param name="onTransitionAsync">Optional async action to execute during transition.</param>
    /// <returns>A new Transition instance.</returns>
    public static Transition Create(
        string actionName,
        string toStateName,
        Func<ITransition, Task> onTransitionAsync = null)
    {
        return new Transition(actionName, toStateName, onTransitionAsync);
    }


}