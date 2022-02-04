using System;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Entities;

public class Order
{
    public OrderState State { get; }
    public Guid Id { get; }
    public double Amount { get; }
    public string Description { get; }
    
    private Order(double amount, string description)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        Description = description;

        State = OrderState.Create();
    }

    public static Order Create(double amount, string description)
    {
        return new Order(amount, description);
    }
}