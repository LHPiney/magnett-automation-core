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
                .OnExitCode(CreateOrder.ExitCode.Created).GoTo(NodeName.PreAuthorizePayment)
            .Build()
            
            .WithNode<PreAuthorizePayment>(NodeName.PreAuthorizePayment)
                .OnExitCode(PreAuthorizePayment.ExitCode.PreAuthorized).GoTo(NodeName.ConfirmPayment)
                .OnExitCode(PreAuthorizePayment.ExitCode.Denied).GoTo(NodeName.CancelPayment)
            .Build()
            
            .WithNode<ConfirmPayment>(NodeName.ConfirmPayment)
                .OnExitCode(ConfirmPayment.ExitCode.Done).GoTo(NodeName.ConfirmOrder)
                .OnExitCode(ConfirmPayment.ExitCode.Failed).GoTo(NodeName.CancelPayment)
            .Build()
 
            .WithNode<ConfirmOrder>(NodeName.ConfirmOrder).Build()     
            
            .WithNode<CancelPayment>(NodeName.CancelPayment)
                .OnExitCode(CancelPayment.ExitCode.Done).GoTo(NodeName.CancelOrder)
            .Build()
            
            .WithNode<CancelOrder>(NodeName.CancelOrder).Build()
            
            .BuildDefinition();
    }
}