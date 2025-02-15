﻿using System.Threading.Tasks;
using Xunit;

using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows;

public class FlowRunnerTest
{
    [Fact]
    public async Task Run_WhenInvoke_ProcessFlow()
    {
        var definition  = SimpleFlowDefinition.GetDefinition();
        var fieldOne    = ContextField<int>.Create("FieldOne");
        var fieldTwo    = ContextField<int>.Create("FieldTwo");
        var fieldResult = ContextField<int>.Create("FieldResult");

        var flowRunner = FlowRunner.Create(definition, Context.Create());
            
        await flowRunner.Start();

        var expected = flowRunner.Context.Value(fieldOne) +
                     flowRunner.Context.Value(fieldTwo);
        
        var calculated = flowRunner.Context.Value(fieldResult);
            
        Assert.Equal(expected, calculated);
    }
}