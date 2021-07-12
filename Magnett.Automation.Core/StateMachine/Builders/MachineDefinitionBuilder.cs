using Magnett.Automation.Core.Common;
using Magnett.Automation.Core.StateMachine.Collections;

namespace Magnett.Automation.Core.StateMachine.Builders
{
    public class MachineDefinitionBuilder
    {
        private readonly StateList _states;
        private IState _initialState;

        private MachineDefinitionBuilder()
        {
            _states = StateList.Create();
        }

        private MachineDefinitionBuilder StoreState(
            IState state, 
            bool isInitialState)
        {
            _states.Add(state.Name, state);

            if (isInitialState)
            {
                _initialState = state;
            }

            return this;
        }
        
        public StateBuilder InitialState(Enumeration state)
        {
            return StateBuilder.Create(state.Name, true, StoreState);
        }

        public StateBuilder InitialState(string name)
        {
            return StateBuilder.Create(name, true, StoreState);
        }

        public StateBuilder AddState(Enumeration state)
        {
            return StateBuilder.Create(state.Name, false, StoreState);
        }
        
        public StateBuilder AddState(string name)
        {
            return StateBuilder.Create(name, false, StoreState);
        }

        public IMachineDefinition BuildDefinition()
        {
            return MachineDefinition.Create(_initialState, _states);
        }

        public static MachineDefinitionBuilder Create()
        {
            return new();
        }
    }
}