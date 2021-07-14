using Xunit;

using Magnett.Automation.Core.StateMachine;
using Magnett.Automation.Core.StateMachine.Collections;

namespace Magnett.Automation.Core.Test.StateMachine
{
    public class MachineDefinitionTest
    {
        private const string InitialStateName = "Start";
        
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