using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Collections;
using Magnett.Automation.Core.StateMachines.Exceptions;
using Magnett.Automation.Core.StateMachines.Implementations;
using Moq;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.StateMachines.Implementations
{
    public class StateTest
    {
        private static readonly CommonNamedKey InitialStateKey = 
            CommonNamedKey.Create("Initial");
        private static readonly CommonNamedKey NotFoundAction = 
            CommonNamedKey.Create("NotFoundAction");
        private static readonly CommonNamedKey AnyAction = 
            CommonNamedKey.Create("AnyAction");
        
        [Fact]
        public void Create_When_Invoke_Return_Instance()
        {
            var instance = State.Create(
                InitialStateKey.Name, 
                TransitionList.Create());

            Assert.NotNull(instance);
        }
        
        [Fact]
        public void Create_When_Invoke_With_Null_TransitionList_Throw_Exception()
        {
            Assert. Throws<ArgumentNullException>(() =>
                State.Create(
                    InitialStateKey.Name,
                    null));
        }
        
        [Fact]
        public void Create_When_Name_Is_Good_Is_Proper_Stored()
        {
            var instance = State.Create(
                InitialStateKey.Name, 
                TransitionList.Create());
            
            Assert.Equal(InitialStateKey.Name, instance.Key.Name);
        }

        [Fact]
        public void ManageAction_When_Action_NotFound_Throws_Exception()
        {
            var transitions = new Mock<TransitionList>();

            transitions
                .Setup(list => list.HasItem(NotFoundAction))
                .Returns(false);
            
            var state = State.Create(InitialStateKey.Name,
                                     transitions.Object);
            
            Assert.Throws<ActionNotFoundException>(() =>
                state.ManageAction(NotFoundAction));
        }

        [Fact] public void ManageAction_When_Action_Is_Found_Call_Transactions_Get()
        {
            var transition = new Mock<ITransition>();
            var transitions = new Mock<TransitionList>();

            transitions
                .Setup(list => list.HasItem(AnyAction))
                .Returns(true);
            
            transitions
                .Setup(list => list.Get(AnyAction))
                .Returns(transition.Object);
            
            var state = State.Create(
                InitialStateKey.Name,
                transitions.Object);

            _ = state.ManageAction(AnyAction);
            
            transitions.Verify(
                dic =>  dic.Get(AnyAction), 
                Times.Once);
        }
        
        [Fact] public void IsFinalState_When_Invoke_Call_Transactions_IsEmpty()
        {
            var transitions = new Mock<TransitionList>();

            var state = State.Create(
                InitialStateKey.Name,
                transitions.Object);

            _ = state.IsFinalState();
            
            transitions.Verify(
                dic =>  dic.IsEmpty(), 
                Times.Once);
        }
    }
}