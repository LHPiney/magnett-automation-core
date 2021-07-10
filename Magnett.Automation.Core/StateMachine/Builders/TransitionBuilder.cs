using System;

namespace Magnett.Automation.Core.StateMachine.Builders
{
    public class TransitionBuilder
    {
        private readonly string _actionName;
        private readonly Func<ITransition, StateBuilder> _storeAction;
        private string _targetStateName;
        
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

        public StateBuilder ToState(string stateName)
        {
            _targetStateName = stateName;

            return _storeAction.Invoke(
                Transition.Create(_actionName, _targetStateName));
        }

        public static TransitionBuilder Create(
            string actionName,
            Func<ITransition, StateBuilder> storeAction)
        {
            return new(actionName, storeAction);
        }
    }
}