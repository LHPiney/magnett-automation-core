using System;
using System.Threading.Tasks;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Entities;

public class Order
{
    private readonly IEventBus _eventBus;

    public OrderState State { get; private set; }
    public Guid Id { get; }
    public double Amount { get; }
    public string Description { get; }
    
    private Order(double amount, string description, IEventBus eventBus)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        Description = description;
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    private async Task<Order> InitStateMachineAsync()
    {
        State = await OrderState.CreateAsync(_eventBus);

        return this;
    }

    public async Task<Order> Cancel()
    {
        await State.DispatchAsync(OrderDefinition.Action.Cancel);

        return this;
    }

    public async Task<Order> Confirm()
    {
        await State.DispatchAsync(OrderDefinition.Action.Confirm);
        
        return this;
    }

    public async Task<Order> Validate()
    {
        if (Amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than 0");
        }

        await State.DispatchAsync(OrderDefinition.Action.Validate);

return this;
    }

    public static async Task<Order> CreateAsync(double amount, string description, IEventBus eventBus)
    {
        return await new Order(amount, description, eventBus)
            .InitStateMachineAsync();
    }
}