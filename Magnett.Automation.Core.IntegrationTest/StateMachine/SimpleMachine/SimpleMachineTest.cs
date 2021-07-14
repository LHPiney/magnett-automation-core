using Xunit;

using Magnett.Automation.Core.IntegrationTest.StateMachine.SimpleMachine.Definitions;

namespace Magnett.Automation.Core.IntegrationTest.StateMachine.SimpleMachine
{
    public class SimpleMachineTest
    {
        [Fact]
        public void CreateDefinition_GetValidDefinition()
        {
            var definition = SimpleMachineDefinition.GetDefinition();
            
            Assert.NotNull(definition);
        }
    }
}