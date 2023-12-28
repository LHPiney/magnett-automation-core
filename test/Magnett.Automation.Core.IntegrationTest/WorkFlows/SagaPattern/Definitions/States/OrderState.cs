using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;

public class OrderState : Machine
{
    private OrderState(IMachineDefinition definition) : base(definition)
    {
    }

    public static OrderState Create()
    {
        return new OrderState(OrderStateDefinition.Definition);
    }

    public override string ToString()
    {
        return State.ToString();
    }
}