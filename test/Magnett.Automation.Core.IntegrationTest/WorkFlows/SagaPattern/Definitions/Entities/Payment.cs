using System;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;
using Magnett.Automation.Core.StateMachines;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Entities;

public class Payment
{
    public Guid Id { get; }
    public double Amount { get; }
    public PaymentState State { get; }

    private Payment(double amount)
    {
        Id = Guid.NewGuid();
        Amount = amount;

        State = PaymentState.Create();
    }

    public static Payment Create(double amount)
    {
        return new Payment(amount);
    }
}