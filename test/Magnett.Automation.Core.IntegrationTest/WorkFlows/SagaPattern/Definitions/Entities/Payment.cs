using System;
using System.Threading.Tasks;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Entities;

public class Payment
{
    private readonly IEventBus _eventBus;
    public Guid Id { get; }
    public double Amount { get; }
    public PaymentState State { get; private set; }


    private Payment(double amount, IEventBus eventBus)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    private async Task<Payment> InitStateMachineAsync()
    {
        State = await PaymentState.CreateAsync(_eventBus);

        return this;
    }

     
    public bool IsPreAuthorized()
    {
        return State.Current.Equals(PaymentDefinition.Status.PreAuthorized);
    }
    
    public async Task<Payment> Confirm()
    {
        await State.DispatchAsync(PaymentDefinition.Action.Confirm);

        return this;
    }
    
    public async Task<Payment> Rollback()
    {
        await State.DispatchAsync(PaymentDefinition.Action.Cancel);
        
        return this;
    }
    
    public async Task<Payment> Cancel()
    {
        await State.DispatchAsync(PaymentDefinition.Action.Cancel);

        return this;
    }

    public async Task<Payment> PreAuthorize()
    {
        await State.DispatchAsync(PaymentDefinition.Action.PreAuthorize);

        return this;
    }
    public static async Task<Payment> CreateAsync(double amount, IEventBus eventBus)
    {
        return await new Payment(amount, eventBus)
            .InitStateMachineAsync();
    }
}