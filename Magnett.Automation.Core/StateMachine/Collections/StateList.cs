using Magnett.Automation.Core.Common;

namespace Magnett.Automation.Core.StateMachine.Collections
{
    public class StateList : DictionaryWrapper<IState>
    {
        public static StateList Create()
        {
            return new();
        }
    }
}