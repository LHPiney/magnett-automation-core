using System;
using Magnett.Automation.Core.Common;
using Magnett.Automation.Core.StateMachine.Collections;

namespace Magnett.Automation.Core.StateMachine.Builders
{
    public class StateBuilder
    {
        private readonly string _stateName;
        private readonly bool _isInitialState;
        private readonly TransitionList _transitions;
        
        private readonly Func<IState, bool, MachineDefinitionBuilder> _storeAction;
        
        public string StateName => _stateName;
        public bool IsInitialState => _isInitialState;
        
        private StateBuilder(
            string stateName,
            bool isInitialState,
            Func<IState, bool, MachineDefinitionBuilder> storeAction)
        {
            if (string.IsNullOrEmpty(stateName))
                throw new ArgumentNullException(nameof(stateName));
                    
            _stateName      = stateName;
            _isInitialState = isInitialState;
            _storeAction    = storeAction;
            _transitions    = TransitionList.Create();
        }

        private StateBuilder StoreTransition(ITransition transition)
        {
            _transitions.Add(transition.ActionKey, transition);

            return this;
        }
        
        public TransitionBuilder OnAction(Enumeration action)
        {
            return TransitionBuilder.Create(action.Name, StoreTransition);
        }        
        
        public TransitionBuilder OnAction(string actionName)
        {
            return TransitionBuilder.Create(actionName, StoreTransition);
        }

        public MachineDefinitionBuilder Build()
        {
            var state = State.Create(_stateName, _transitions);

            return _storeAction.Invoke(state, _isInitialState);
        }

        public static StateBuilder Create(
            string stateName,
            bool isInitialState,
            Func<IState, bool, MachineDefinitionBuilder> storeAction)
        {
            return new (stateName, isInitialState, storeAction);
        }
    }
}