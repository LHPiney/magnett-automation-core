using Magnett.Automation.Core.WorkFlows;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Nodes;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Runtimes.Builders;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions;

public static class SagaPatternDefinition
{
    public static IFlowDefinition Definition { get; }
    
    static SagaPatternDefinition()
    {
        Definition = FlowDefinitionBuilder.Create()
            .WithInitialNode(CreateOrder.Create(NodeName.CreateOrder))
                .OnExitCode(CreateOrder.ExitCode.Created).GoTo(NodeName.PreAuthorizePayment)
            .Build()
            
            .WithNode(PreAuthorizePayment.Create(NodeName.PreAuthorizePayment))
                .OnExitCode(PreAuthorizePayment.ExitCode.PreAuthorized).GoTo(NodeName.ConfirmPayment)
                .OnExitCode(PreAuthorizePayment.ExitCode.Denied).GoTo(NodeName.CancelPayment)
            .Build()
            
            .WithNode(ConfirmPayment.Create(NodeName.ConfirmPayment))
                .OnExitCode(ConfirmPayment.ExitCode.Done).GoTo(NodeName.ConfirmOrder)
                .OnExitCode(ConfirmPayment.ExitCode.Failed).GoTo(NodeName.CancelPayment)
            .Build()
 
            .WithNode(ConfirmOrder.Create(NodeName.ConfirmOrder)).Build()     
            
            .WithNode(CancelPayment.Create(NodeName.CancelPayment))
                .OnExitCode(CancelPayment.ExitCode.Done).GoTo(NodeName.CancelOrder)
            .Build()
            
            .WithNode(CancelOrder.Create(NodeName.CancelOrder)).Build()
            
            .BuildDefinition();
    }
}