using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Builders;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.PaymentMachine;

/// <summary>
/// State machine definition for payment processing workflow.
/// This demonstrates how to define a complete state machine with all possible transitions.
/// </summary>
public static class PaymentStateMachineDefinition
{
    public static IMachineDefinition Definition { get; } = MachineDefinitionBuilder.Create()
        .InitialState(PaymentState.Pending)
            .OnAction(PaymentAction.Process).ToState(PaymentState.Processing)
            .OnAction(PaymentAction.Cancel).ToState(PaymentState.Cancelled)
            .Build()
        .AddState(PaymentState.Processing)
            .OnAction(PaymentAction.Complete).ToState(PaymentState.Completed)
            .OnAction(PaymentAction.Fail).ToState(PaymentState.Failed)
            .OnAction(PaymentAction.Retry).ToState(PaymentState.Pending)
            .Build()
        .AddState(PaymentState.Completed)
            .OnAction(PaymentAction.Refund).ToState(PaymentState.Refunded)
            .Build()
        .AddState(PaymentState.Failed)
            .OnAction(PaymentAction.Retry).ToState(PaymentState.Pending)
            .OnAction(PaymentAction.Cancel).ToState(PaymentState.Cancelled)
            .Build()
        .AddState(PaymentState.Cancelled)
            .Build()
        .AddState(PaymentState.Refunded)
            .Build()
        .BuildDefinition();
}
