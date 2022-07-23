using Xunit;

using Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows;

public class SimpleFlowTest
{
    [Fact]
    public void CreateDefinition_GetValidDefinition()
    {
        var definition = SimpleFlowDefinition.GetDefinition();
            
        Assert.NotNull(definition);
    }
}