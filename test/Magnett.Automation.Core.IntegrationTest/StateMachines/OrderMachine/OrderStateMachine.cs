using System.Threading.Tasks;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.OrderMachine;

/// <summary>
/// Order state machine implementation that inherits from Machine.
/// This demonstrates the recommended pattern for creating state machines.
/// </summary>
public sealed class OrderStateMachine : Machine
{
    private OrderStateMachine(IMachineDefinition definition, IEventBus eventBus) : base(definition, eventBus)
    {
        
    }

    public static async Task<OrderStateMachine> CreateAsync(IEventBus eventBus)
    {
        var machine = new OrderStateMachine(OrderStateMachineDefinition.Definition, eventBus);
        await machine.InitializeAsync();
        return machine;
    }

    /// <summary>
    /// Gets the current state as an OrderState enumeration.
    /// This provides a type-safe way to access the current state.
    /// </summary>
    public OrderState CurrentOrderState => this switch
    {
        var machine when machine.Equals(OrderState.Pending) => OrderState.Pending,
        var machine when machine.Equals(OrderState.Confirmed) => OrderState.Confirmed,
        var machine when machine.Equals(OrderState.Processing) => OrderState.Processing,
        var machine when machine.Equals(OrderState.Shipped) => OrderState.Shipped,
        var machine when machine.Equals(OrderState.Delivered) => OrderState.Delivered,
        var machine when machine.Equals(OrderState.Cancelled) => OrderState.Cancelled,
        var machine when machine.Equals(OrderState.Returned) => OrderState.Returned,
        _ => OrderState.Pending
    };
}
