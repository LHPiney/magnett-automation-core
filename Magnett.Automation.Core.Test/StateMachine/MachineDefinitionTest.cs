using Xunit;

using Magnett.Automation.Core.StateMachine;
using Magnett.Automation.Core.StateMachine.Collections;
using Magnett.Automation.Core.StateMachine.Exceptions;

namespace Magnett.Automation.Core.Test.StateMachine
{
    public class MachineDefinitionTest
    {
        private const string InitialStateName = "Start";

        private static StateList CreateStateList()
        {
            var result = new StateList();
            
            result.Add(InitialStateName, State.Create(InitialStateName));
            
            return result;
        }
        
        [Fact]
        public void Create_When_Invoke_Return_Instance()
        {
            var initialState = State.Create(InitialStateName); 
            
            var stateList = new StateList();
            
            stateList.Add(initialState.Name, initialState);
            
            var definition = MachineDefinition.Create(initialState, stateList);
            
            Assert.NotNull(definition);
        }
    }
}