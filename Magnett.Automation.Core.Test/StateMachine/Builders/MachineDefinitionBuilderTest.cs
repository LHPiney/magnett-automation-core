using System;
using Xunit;

using Magnett.Automation.Core.StateMachine.Builders;
using Magnett.Automation.Core.StateMachine.Exceptions;

namespace Magnett.Automation.Core.Test.StateMachine.Builders
{
    public class MachineDefinitionBuilderTest
    {
        private const string InitialState = "Initial";

        [Fact]
        public void Create_When_Invoke_Return_Instance()
        {
            var instance = MachineDefinitionBuilder.Create();

            Assert.NotNull(instance);
        }
        
        
    }
}