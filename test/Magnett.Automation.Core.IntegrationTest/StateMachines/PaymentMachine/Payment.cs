#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magnett.Automation.Core.Events;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.PaymentMachine;

/// <summary>
/// Payment entity that encapsulates a state machine for managing payment lifecycle.
/// This demonstrates how to embed a state machine within a domain entity and expose
/// business methods that dispatch actions on the internal state machine.
/// </summary>
public class Payment
{
    private readonly IEventBus _eventBus;
    private PaymentStateMachine? _machine;
    
    public string Id { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public string PaymentMethod { get; }
    public PaymentState CurrentState => _machine?.CurrentPaymentState ?? PaymentState.Pending;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public string? TransactionId { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int RetryCount { get; private set; }

    public Payment(string id, decimal amount, string currency, string paymentMethod, IEventBus eventBus)
    {
        Id = id;
        Amount = amount;
        Currency = currency;
        PaymentMethod = paymentMethod;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        RetryCount = 0;
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public async Task InitializeAsync()
    {
        _machine = await PaymentStateMachine.CreateAsync(_eventBus);
        UpdatedAt = DateTime.UtcNow;
    }

    public async Task<bool> ProcessAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Payment not initialized");
        
        try
        {
            await _machine.DispatchAsync(PaymentAction.Process);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CompleteAsync(string transactionId)
    {
        if (_machine == null) throw new InvalidOperationException("Payment not initialized");
        
        try
        {
            await _machine.DispatchAsync(PaymentAction.Complete);
            TransactionId = transactionId;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> FailAsync(string errorMessage)
    {
        if (_machine == null) throw new InvalidOperationException("Payment not initialized");
        
        try
        {
            await _machine.DispatchAsync(PaymentAction.Fail);
            ErrorMessage = errorMessage;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> RetryAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Payment not initialized");
        
        try
        {
            await _machine.DispatchAsync(PaymentAction.Retry);
            RetryCount++;
            ErrorMessage = null; // Clear previous error
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CancelAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Payment not initialized");
        
        try
        {
            await _machine.DispatchAsync(PaymentAction.Cancel);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> RefundAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Payment not initialized");
        
        try
        {
            await _machine.DispatchAsync(PaymentAction.Refund);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
