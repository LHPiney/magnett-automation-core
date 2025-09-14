using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Codes;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Nodes;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Definitions.Builders;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions;

public static class SagaPatternDefinition
{   
    public static IFlowDefinition Definition { get; }
    
    static SagaPatternDefinition()
    {
        Definition = FlowDefinitionBuilder.Create()
            .WithInitialNode<CreateOrder>(NodeName.CreateOrder)
                .OnExitCode(ExitCode.Created).GoTo(NodeName.PreAuthorizePayment)
                .OnExitCode(ExitCode.Canceled).GoTo(NodeName.CancelOrder)
                .Build()
            
            .WithNode<PreAuthorizePayment>(NodeName.PreAuthorizePayment)
                .OnExitCode(ExitCode.PreAuthorized).GoTo(NodeName.ConfirmPayment)
                .OnExitCode(ExitCode.Denied).GoTo(NodeName.CancelPayment)
                .OnExitCode(ExitCode.Canceled).GoTo(NodeName.CancelPayment)
                .Build()
            
            .WithNode<ConfirmPayment>(NodeName.ConfirmPayment)
                .OnExitCode(ExitCode.Done).GoTo(NodeName.ConfirmOrder)
                .OnExitCode(ExitCode.Failed).GoTo(NodeName.CancelPayment)
                .OnExitCode(ExitCode.Canceled).GoTo(NodeName.CancelPayment)
                .Build()
 
            .WithNode<ConfirmOrder>(NodeName.ConfirmOrder).Build()     
            
            .WithNode<CancelPayment>(NodeName.CancelPayment)
                .OnExitCode(ExitCode.Done).GoTo(NodeName.CancelOrder)
                .Build()
            
            .WithNode<CancelOrder>(NodeName.CancelOrder).Build()
            
            .BuildDefinition();
    }
}