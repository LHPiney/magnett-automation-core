namespace Magnett.Automation.Core.StateMachine.Collections
{
    public class TransitionList : DictionaryWrapper<ITransition>
    {
        public static TransitionList Create()
        {
            return new ();
        }
    }
}