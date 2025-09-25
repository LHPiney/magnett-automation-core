using System.Threading.Tasks;
using Xunit;

using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;
using Moq;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows;

public class FlowRunnerTest
{
    [Fact]
    public async Task Run_WhenInvoke_ProcessFlow()
    {
        var eventBus    = new Mock<IEventBus>();
        var definition  = SimpleFlowDefinition.GetDefinition();
        var fieldOne    = ContextField<int>.WithName("FieldOne");
        var fieldTwo    = ContextField<int>.WithName("FieldTwo");
        var fieldResult = ContextField<int>.WithName("FieldResult");

        var flowRunner = FlowRunner.Create(definition, Context.Create(eventBus.Object));
            
        await flowRunner.Start();

        var expected = flowRunner.Context.Value(fieldOne) + flowRunner.Context.Value(fieldTwo);
        
        var calculated = flowRunner.Context.Value(fieldResult);
            
        Assert.Equal(expected, calculated);
    }
}