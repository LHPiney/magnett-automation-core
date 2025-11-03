using System;
using System.Text.Json;
using System.Threading.Tasks;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Contexts.Serializations;
using Magnett.Automation.Core.Contexts.Serializations.Attributes;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Entities;

[ContextSerializable]
public class Payment : IContextSerializable
{
    private IEventBus? _eventBus;

    public Guid Id { get; private set; }
    public double Amount { get; private set; }
    public PaymentState? State { get; private set; }

    private Payment()
    {
    }

    private Payment(double amount, IEventBus eventBus)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    private async Task<Payment> InitStateMachineAsync()
    {
        if (_eventBus != null)
        {
            State = await PaymentState.CreateAsync(_eventBus);
        }

        return this;
    }
     
    public bool IsPreAuthorized()
    {
        return State?.Current.Equals(PaymentDefinition.Status.PreAuthorized) ?? false;
    }
    
    public async Task<Payment> Confirm()
    {
        if (State != null)
        {
            await State.DispatchAsync(PaymentDefinition.Action.Confirm);
        }

        return this;
    }
    
    public async Task<Payment> Rollback()
    {
        if (State != null)
        {
            await State.DispatchAsync(PaymentDefinition.Action.Cancel);
        }
        
        return this;
    }
    
    public async Task<Payment> Cancel()
    {
        if (State != null)
        {
            await State.DispatchAsync(PaymentDefinition.Action.Cancel);
        }

        return this;
    }

    public async Task<Payment> PreAuthorize()
    {
        if (State != null)
        {
            await State.DispatchAsync(PaymentDefinition.Action.PreAuthorize);
        }

        return this;
    }
    
    public static async Task<Payment> CreateAsync(double amount, IEventBus eventBus)
    {
        return await new Payment(amount, eventBus)
            .InitStateMachineAsync();
    }

    public ContextFieldSnapshot Serialize(string fieldName, ISerializationContext context)
    {
        var data = new PaymentData
        {
            Id = Id,
            Amount = Amount
        };

        return new ContextFieldSnapshot
        {
            FieldName = fieldName,
            FieldType = typeof(Payment).AssemblyQualifiedName ?? "Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Entities.Payment",
            Value = null,
            SerializedValue = JsonSerializer.Serialize(data)
        };
    }

    public static object Deserialize(ContextFieldSnapshot snapshot, ISerializationContext context)
    {
        if (string.IsNullOrEmpty(snapshot.SerializedValue))
        {
            throw new InvalidOperationException("Cannot deserialize Payment: SerializedValue is null or empty");
        }

        var data = JsonSerializer.Deserialize<PaymentData>(snapshot.SerializedValue);
        if (data == null)
        {
            throw new InvalidOperationException("Failed to deserialize Payment data");
        }

        var payment = new Payment
        {
            Id = data.Id,
            Amount = data.Amount,
            _eventBus = context.EventBus
        };

        return payment;
    }

    private class PaymentData
    {
        public Guid Id { get; set; }
        public double Amount { get; set; }
    }
}