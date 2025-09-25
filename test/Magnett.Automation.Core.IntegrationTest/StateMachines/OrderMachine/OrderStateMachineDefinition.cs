using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Builders;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.OrderMachine;

/// <summary>
/// State machine definition for order management workflow.
/// This demonstrates how to define a complete state machine with all possible transitions.
/// </summary>
public static class OrderStateMachineDefinition
{
    public static IMachineDefinition Definition { get; } = MachineDefinitionBuilder.Create()
        .InitialState(OrderState.Pending)
            .OnAction(OrderAction.Confirm).ToState(OrderState.Confirmed)
            .OnAction(OrderAction.Cancel).ToState(OrderState.Cancelled)
            .Build()
        .AddState(OrderState.Confirmed)
            .OnAction(OrderAction.StartProcessing).ToState(OrderState.Processing)
            .OnAction(OrderAction.Cancel).ToState(OrderState.Cancelled)
            .Build()
        .AddState(OrderState.Processing)
            .OnAction(OrderAction.Ship).ToState(OrderState.Shipped)
            .OnAction(OrderAction.Cancel).ToState(OrderState.Cancelled)
            .Build()
        .AddState(OrderState.Shipped)
            .OnAction(OrderAction.Deliver).ToState(OrderState.Delivered)
            .Build()
        .AddState(OrderState.Delivered)
            .OnAction(OrderAction.Return).ToState(OrderState.Returned)
            .Build()
        .AddState(OrderState.Cancelled)
            .Build()
        .AddState(OrderState.Returned)
            .Build()
        .BuildDefinition();
}
