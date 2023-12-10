using Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Nodes;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Definitions.Builders;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions;

public static class SimpleFlowDefinition
{
    private static IFlowDefinition   _definition;
        
    static SimpleFlowDefinition()
    {
        CreateDefinition();
    }
        
    private static void CreateDefinition()
    {
        _definition = FlowDefinitionBuilder.Create()
            .WithInitialNode<ResetValue>(NodeName.Reset)
            .OnExitCode(ResetValue.ExitCode.Ok).GoTo(NodeName.SetValue)
            .Build()
                
            .WithNode<SetValue>(NodeName.SetValue)
            .OnExitCode(SetValue.ExitCode.Assigned).GoTo(NodeName.SumValue)
            .Build()
                
            .WithNode<SumValue>(NodeName.SumValue)
            .Build()
                
            .BuildDefinition();
    }

    public static IFlowDefinition GetDefinition()
    {
        return _definition;
    }
}