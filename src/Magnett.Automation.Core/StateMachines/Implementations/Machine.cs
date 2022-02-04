using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.StateMachines.Exceptions;

namespace Magnett.Automation.Core.StateMachines.Implementations
{
    public class Machine : IMachine
    {
        protected readonly IMachineDefinition _definition;

        protected Machine(IMachineDefinition definition)
        {
            _definition = definition
                          ?? throw new ArgumentNullException(nameof(definition));
            
            Init();
        }

        private void Transit(ITransition transition)
        {
            var newState = _definition.HasState(transition.ToStateKey)
                ? _definition.GetState(transition.ToStateKey)
                : throw new StateNotFoundException(transition.ToStateKey.Name);

            State = newState;
        }
        
        private void Init()
        {
            State = _definition.InitialState
                ?? throw new InvalidMachineDefinitionException("No initial state found");
        }

        # region IMachine
        
        public IMachine Dispatch(Enumeration action)
        {
            return Dispatch(action.Name);
        }
        
        public IMachine Dispatch(string actionName)
        {
            Transit(State
                .ManageAction(CommonNamedKey.Create(actionName)));
            
            return this;
        }

        public IState State { get; private set; }
        
        public bool Equals(CommonNamedKey obj)
        {
            return State.Key.Equals(obj);
        }
        
        public bool Equals(Enumeration obj)
        {
            return State.Key.Equals(obj);
        }

        #endregion
        
        
        public static IMachine Create(IMachineDefinition definition)
        {
            return new Machine(definition);
        }
    }
}