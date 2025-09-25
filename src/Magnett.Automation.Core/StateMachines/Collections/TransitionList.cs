namespace Magnett.Automation.Core.StateMachines.Collections;

public class TransitionList : DictionaryWrapper<ITransition>
{
    /// <summary>
    /// Creates a new TransitionList instance.
    /// </summary>
    /// <returns>A new TransitionList instance.</returns>
    public static TransitionList Create()
    {
        return new TransitionList();
    }
}