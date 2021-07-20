using System;
using Xunit;
using Moq;

using Magnett.Automation.Core.StateMachine;
using Magnett.Automation.Core.StateMachine.Collections;
using Magnett.Automation.Core.StateMachine.Exceptions;

namespace Magnett.Automation.Core.Test.StateMachine
{
    public class StateTest
    {
        private const string InitialStateName   = "Initial";
        private const string NotFoundActionName = "NotFoundAction";
        private const string AnyActionName      = "AnyAction";
        private const string TargetStateName    = "TargetState";

        [Fact]
        public void Create_When_Invoke_Return_Instance()
        {
            var instance = State.Create(InitialStateName, TransitionList.Create());

            Assert.NotNull(instance);
        }

        [Fact]
        public void Create_When_Invoke_With_Null_Name_Throw_Exception()
        {
            Assert. Throws<ArgumentNullException>(() =>
                State.Create(null, TransitionList.Create()));
        }
        
        [Fact]
        public void Create_When_Invoke_With_Null_Transition_Throw_Exception()
        {
            Assert. Throws<ArgumentNullException>(() =>
                State.Create(InitialStateName,null));
        }
        
        [Fact]
        public void Create_When_Name_Is_Good_Is_Proper_Stored()
        {
            var instance = State.Create(InitialStateName, TransitionList.Create());
            
            Assert.Equal(InitialStateName, instance.Name);
        }

        [Fact]
        public void ManageAction_When_Action_NotFound_Throws_Exception()
        {
            var transitions = new Mock<TransitionList>();

            transitions
                .Setup(list => list.HasItem(NotFoundActionName))
                .Returns(false);
            
            var state = State.Create(InitialStateName,
                                     transitions.Object);
            
            Assert.Throws<ActionNotFoundException>(() =>
                state.ManageAction(NotFoundActionName));
        }

        [Fact] public void ManageAction_When_Action_Is_Found_Call_Transactions_Get()
        {
            var transition = new Mock<ITransition>();
            var transitions = new Mock<TransitionList>();

            transitions
                .Setup(list => list.HasItem(AnyActionName))
                .Returns(true);
            
            transitions
                .Setup(list => list.GetItem(AnyActionName))
                .Returns(transition.Object);
            
            var state = State.Create(InitialStateName,
                transitions.Object);

            _ = state.ManageAction(AnyActionName);
            
            transitions.Verify(
                dic =>  dic.GetItem(AnyActionName), 
                Times.Once);
        }
        
        [Fact] public void IsFinalState_When_Invoke_Call_Transactions_IsEmpty()
        {
            var transitions = new Mock<TransitionList>();

            var state = State.Create(InitialStateName,
                transitions.Object);

            _ = state.IsFinalState();
            
            transitions.Verify(
                dic =>  dic.IsEmpty(), 
                Times.Once);
        }
    }
}