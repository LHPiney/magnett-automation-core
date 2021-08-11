using Magnett.Automation.Core.IntegrationTest.StateMachines.SimpleMachine.Definitions;
using Xunit;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.SimpleMachine
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