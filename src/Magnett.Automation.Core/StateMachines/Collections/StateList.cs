namespace Magnett.Automation.Core.StateMachines.Collections;

public class StateList : DictionaryWrapper<IState>
{
    /// <summary>
    /// Creates a new StateList instance.
    /// </summary>
    /// <returns>A new StateList instance.</returns>
    public static StateList Create()
    {
        return new StateList();
    }
}