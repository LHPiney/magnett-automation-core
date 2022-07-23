namespace Magnett.Automation.Core.StateMachines.Collections;

public class StateList : DictionaryWrapper<IState>
{
    public static StateList Create()
    {
        return new();
    }
}