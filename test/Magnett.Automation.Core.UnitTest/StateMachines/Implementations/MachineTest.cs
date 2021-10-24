using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Exceptions;
using Magnett.Automation.Core.StateMachines.Implementations;
using Moq;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.StateMachines
{
    public class MachineTest
    {
        private static readonly CommonNamedKey InitialStateKey  = 
            CommonNamedKey.Create("InitialState");
        private static readonly CommonNamedKey FinalStateKey    = 
            CommonNamedKey.Create("FinalState");
        private static readonly CommonNamedKey NotFoundStateKey = 
            CommonNamedKey.Create("NotFound");
        
        [Fact]
        public void Create_When_Invoke_Return_Instance()
        {
            var definition = new Mock<IMachineDefinition>();
            var state      = new Mock<IState>();

            definition
                .Setup(def => def.InitialState)
                .Returns(state.Object);
            
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
                .Setup(tra => tra.ToStateKey)
                .Returns(NotFoundStateKey);

            initialState
                .Setup(sta => sta.ManageAction(It.IsAny<CommonNamedKey>()))
                .Returns(transition.Object);
            
            definition
                .Setup(def => def.InitialState)
                .Returns(initialState.Object);

            definition
                .Setup(def => 
                    def.HasState(It.Is<CommonNamedKey>(val => val.Equals(NotFoundStateKey))))
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
                .Setup(tra => tra.ToStateKey)
                .Returns(FinalStateKey);

            initialState
                .Setup(sta => sta.ManageAction(It.IsAny<CommonNamedKey>()))
                .Returns(transition.Object);
            
            initialState
                .Setup(sta => sta.Key)
                .Returns(InitialStateKey);
            
            finalState
                .Setup(sta => sta.Key)
                .Returns(FinalStateKey);
            
            definition
                .Setup(def => def.InitialState)
                .Returns(initialState.Object);

            definition
                .Setup(def => 
                    def.HasState(It.Is<CommonNamedKey>(val => val.Equals(FinalStateKey))))
                .Returns(true);
            
            definition
                .Setup(def => 
                    def.GetState(It.Is<CommonNamedKey>(val => val.Equals(FinalStateKey))))
                .Returns(finalState.Object);
            
            var machine = Machine.Create(definition.Object);
                
            var firstStateName = machine.State.Key;
            machine.Dispatch("AnyAction");
            var secondStateName = machine.State.Key;
            
            Assert.NotNull(machine.State);
            Assert.NotEqual(firstStateName, secondStateName);
            Assert.Equal(FinalStateKey, secondStateName);
        }
    }
}