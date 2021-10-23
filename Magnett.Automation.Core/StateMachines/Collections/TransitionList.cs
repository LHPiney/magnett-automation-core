using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.StateMachines.Collections
{
    public class TransitionList : DictionaryWrapper<ITransition>
    {
        public static TransitionList Create()
        {
            return new();
        }
    }
}