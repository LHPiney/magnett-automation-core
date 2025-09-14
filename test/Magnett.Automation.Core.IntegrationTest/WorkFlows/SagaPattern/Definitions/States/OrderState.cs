using System.Threading.Tasks;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;

public sealed class OrderState : Machine
{
    private OrderState(IMachineDefinition definition, IEventBus eventBus) : base(definition, eventBus)
    {
    }

    public static async Task<OrderState> CreateAsync(IEventBus eventBus)
    {
        var orderState = new OrderState(OrderDefinition.Definition, eventBus);

        await orderState.InitializeAsync();

        return orderState;
    }

    public override string ToString()
    {
        return Current.ToString();
    }
}