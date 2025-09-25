namespace Magnett.Automation.Core.StateMachines;

/// <summary>
/// Defines a transition in a state machine, linking an action to a target state.
/// </summary>
public interface ITransition
{
    public CommonNamedKey ToStateKey { get; }
    public CommonNamedKey ActionKey { get; }
    public Func<ITransition, Task>? OnTransitionAsync { get; }
}