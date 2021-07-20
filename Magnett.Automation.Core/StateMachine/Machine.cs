using System;

using Magnett.Automation.Core.StateMachine.Exceptions;

namespace Magnett.Automation.Core.StateMachine
{
    public class Machine : IMachine
    {
        private readonly IMachineDefinition _definition;

        private Machine(IMachineDefinition definition)
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

            State = newState;
        }
        
        private void Init()
        {
            State = _definition.InitialState
                ?? throw new InvalidMachineDefinitionException("No initial state found");
        }

        # region IMachine
        public IMachine Dispatch(string actionName)
        {
            Transit(State.ManageAction(actionName));
            
            return this;
        }

        public IState State { get; private set; }

        #endregion

        public static IMachine Create(IMachineDefinition definition)
        {
            return new Machine(definition);
        }
    }
}