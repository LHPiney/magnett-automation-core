using System;
using Moq;
using Xunit;

using Magnett.Automation.Core.StateMachine;
using Magnett.Automation.Core.StateMachine.Exceptions;

namespace Magnett.Automation.Core.Test.StateMachine
{
    public class MachineTest
    {
        private const string InitialStateName  = "InitialState";
        private const string FinalStateName    = "FinalState";
        private const string NotFoundStateName = "NotFound";
        
        [Fact]
        public void Create_When_Invoke_Return_Instance()
        {
            var definition = new Mock<IMachineDefinition>();
            var state      = new Mock<IState>();

            definition
                .Setup(def => def.InitialState).Returns(state.Object);
            
            var instance = Machine.Create(definition.Object);
            
            Assert.NotNull(instance);
        }
        
        [Fact]
        public void Create_When_Invoke_With_Null_Definition_Throw_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Machine.Create(null));
        }
        
        [Fact]
        public void Create_When_Invoke_Without_InitialState_Throw_Exception()
        {
            var definition = new Mock<IMachineDefinition>();

            definition
                .Setup(def => def.InitialState)
                .Returns<IState>(null);
            
            Assert.Throws<InvalidMachineDefinitionException>(() =>
                Machine.Create(definition.Object));
        }
        
        [Fact]
        public void Dispatch_When_Transition_Has_Unknown_State_Throw_Exception()
        {
            var definition   = new Mock<IMachineDefinition>();
            var transition   = new Mock<ITransition>();
            var initialState = new Mock<IState>();

            transition
                .Setup(tra => tra.ToStateName)
                .Returns(NotFoundStateName);

            initialState
                .Setup(sta => sta.ManageAction(It.IsAny<string>()))
                .Returns(transition.Object);
            
            definition
                .Setup(def => def.InitialState)
                .Returns(initialState.Object);

            definition
                .Setup(def => 
                    def.HasState(It.Is<string>(val => val.Equals(NotFoundStateName))))
                .Returns(false);

            var machine = Machine.Create(definition.Object);
            Assert.Throws<StateNotFoundException>(() =>
                machine.Dispatch("AnyAction"));
        }

        [Fact] public void Dispatch_When_Transition_Is_Valid_Change_State()
        {
            var definition   = new Mock<IMachineDefinition>();
            var transition   = new Mock<ITransition>();
            var initialState = new Mock<IState>();
            var finalState   = new Mock<IState>();
            
            transition
                .Setup(tra => tra.ToStateName)
                .Returns(FinalStateName);

            initialState
                .Setup(sta => sta.ManageAction(It.IsAny<string>()))
                .Returns(transition.Object);
            
            initialState
                .Setup(sta => sta.Name)
                .Returns(InitialStateName);
            
            finalState
                .Setup(sta => sta.Name)
                .Returns(FinalStateName);
            
            definition
                .Setup(def => def.InitialState)
                .Returns(initialState.Object);

            definition
                .Setup(def => 
                    def.HasState(It.Is<string>(val => val.Equals(FinalStateName))))
                .Returns(true);
            
            definition
                .Setup(def => 
                    def.GetState(It.Is<string>(val => val.Equals(FinalStateName))))
                .Returns(finalState.Object);
            
            var machine = Machine.Create(definition.Object);
                
            var firstStateName = machine.State.Name;
            machine.Dispatch("AnyAction");
            var secondStateName = machine.State.Name;
            
            Assert.NotNull(machine.State);
            Assert.NotEqual(firstStateName, secondStateName);
            Assert.Equal(FinalStateName, secondStateName);
        }
    }
}