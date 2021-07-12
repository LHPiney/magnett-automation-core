using Xunit;

using Magnett.Automation.Core.Test.Integration.SimpleMachine.Definitions;

namespace Magnett.Automation.Core.Test.Integration.SimpleMachine
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