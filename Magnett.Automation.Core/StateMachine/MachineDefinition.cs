using System;
using System.Runtime.CompilerServices;

using Magnett.Automation.Core.StateMachine.Collections;

[assembly: InternalsVisibleTo("Magnett.Automation.Core.Test")]
namespace Magnett.Automation.Core.StateMachine
{
    internal class MachineDefinition : IMachineDefinition
    {
        private readonly StateList _states; 
        
        private MachineDefinition(
            IState initialState, 
            StateList stateList)
        {
            InitialState = initialState ??
                           throw new ArgumentNullException(nameof(initialState));
                           
            _states = stateList ??
                      throw new ArgumentNullException(nameof(stateList));
        }
        
        #region IMachineDefinition

        public IState InitialState { get; }
        
        public bool HasState(string stateName)
        {
            return _states.HasItem(stateName);
        }

        public IState GetState(string stateName)
        {
            return _states.Get(stateName);
        }

        #endregion
        
        public static IMachineDefinition Create(
            IState initialState,
            StateList stateList)
        {
            return new MachineDefinition(initialState, stateList);
        }
    }
}