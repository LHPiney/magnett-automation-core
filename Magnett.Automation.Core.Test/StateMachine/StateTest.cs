using Xunit;

using Magnett.Automation.Core.StateMachine;
using Magnett.Automation.Core.StateMachine.Exceptions;

namespace Magnett.Automation.Core.Test.StateMachine
{
    public class StateTest
    {
        private const string InitialStateName  = "Init";
        private const string PreparedStateName = "Prepared";
        private const string PrepareActionName = "Prepare";
            
        [Fact]
        public void Create_When_Invoke_Return_Instance()
        {
            var instance = State.Create(InitialStateName);

            Assert.NotNull(instance);
        }

        [Fact]
        public void Create_When_Name_Is_Good_Is_Proper_Stored()
        {
            var instance = State.Create(InitialStateName);
            
            Assert.Equal(InitialStateName, instance.Name);
        }
    }
}