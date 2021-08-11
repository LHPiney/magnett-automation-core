using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.StateMachines.Collections;

[assembly: InternalsVisibleTo("Magnett.Automation.Core.Test")]
namespace Magnett.Automation.Core.StateMachines
{
    internal class MachineDefinition : IMachineDefinition
    {
        private readonly StateList _stateList; 
        
        private MachineDefinition(
            IState initialState, 
            StateList stateList)
        {
            InitialState = initialState ??
                           throw new ArgumentNullException(nameof(initialState));
                           
            _stateList = stateList ??
                      throw new ArgumentNullException(nameof(stateList));
        }
        
        #region IMachineDefinition

        public IState InitialState { get; }
        
        public bool HasState(CommonNamedKey stateKey)
        {
            return _stateList.HasItem(stateKey);
        }

        public IState GetState(CommonNamedKey stateKey)
        {
            return _stateList.Get(stateKey);
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