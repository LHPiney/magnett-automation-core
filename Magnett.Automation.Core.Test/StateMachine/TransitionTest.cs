using System;
using Xunit;

using Magnett.Automation.Core.StateMachine;
    
namespace Magnett.Automation.Core.Test.StateMachine
{
    public class TransitionTest
    {
        private const string ActionName = "Action";
        private const string StateName  = "State";
            
        [Fact]
        public void Create_When_Invoke_Return_Instance()
        {
            var instance = Transition.Create(ActionName, StateName);
            
            Assert.NotNull(instance);
        }

        [Fact]
        public void Create_When_Instance_Is_Created_Action_Return_Correct_Value()
        {
            var instance = Transition.Create(ActionName, StateName);
            
            Assert.Equal(ActionName, instance.Action);
        }
        
        [Fact]
        public void Create_When_Instance_Is_Created_ToStateName_Return_Correct_Value()
        {
            var instance = Transition.Create(ActionName, StateName);
            
            Assert.Equal(StateName, instance.ToStateName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Create_When_Invoke_With_NullOrEmpty_Action_Throw_Exception(string action)
        {
            Assert.Throws<ArgumentNullException>(() =>
                Transition.Create(action, StateName));
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Create_When_Invoke_With_NullOrEmpty_TargetStateName_Throw_Exception(string stateName)
        {
            Assert.Throws<ArgumentNullException>(() =>
                Transition.Create(ActionName, stateName));
        }
    }
}