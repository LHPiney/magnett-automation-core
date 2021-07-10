using System;

using Magnett.Automation.Core.StateMachine.Exceptions;

namespace Magnett.Automation.Core.StateMachine
{
    public class Machine : IMachine
    {
        private readonly IMachineDefinition _definition;
        private IState _state;

        protected Machine(IMachineDefinition definition)
        {
            _definition = definition
                          ?? throw new ArgumentNullException(nameof(definition));
            
            Init();
        }

        private void Transit(ITransition transition)
        {
            var newState = _definition.HasState(transition.ToStateName)
                ? _definition.GetState(transition.ToStateName)
                : throw new StateNotFoundException(transition.ToStateName);

            _state = newState;
        }
        
        private void Init()
        {
            _state = _definition.InitialState;
        }

        # region IMachine
        public IMachine Dispatch(string actionName)
        {
            if (_state == null) throw new UndefinedStateException();

            var transition = _state.ManageAction(actionName);
            
            return this;
        }

        public IState State()
        {
            return _state;
        }
        #endregion

        public static IMachine Create(IMachineDefinition definition)
        {
            return new Machine(definition);
        }
    }
}