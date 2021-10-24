using System;
using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.StateMachines.Builders
{
    public class 
    TransitionBuilder
    {
        private readonly string _actionName;
        private readonly Func<ITransition, StateBuilder> _storeAction;
        
        private TransitionBuilder(
            string actionName, 
            Func<ITransition, StateBuilder> storeAction)
        {
            if (string.IsNullOrEmpty(actionName))
                throw new ArgumentNullException(nameof(actionName));

            _actionName = actionName;
            _storeAction = storeAction ??
                           throw new ArgumentNullException(nameof(storeAction));
        }

        public StateBuilder ToState(Enumeration state)
        {
            return ToState(state.Name);
        }
        
        public StateBuilder ToState(string stateName)
        {
            return _storeAction.Invoke(
                Transition.Create(_actionName, stateName));
        }
        
        public static TransitionBuilder Create(
            string actionName,
            Func<ITransition, StateBuilder> storeAction)
        {
            return new(actionName, storeAction);
        }
    }
}