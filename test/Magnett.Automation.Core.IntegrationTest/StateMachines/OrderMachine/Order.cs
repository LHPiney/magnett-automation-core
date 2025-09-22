#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magnett.Automation.Core.Events;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.OrderMachine;

/// <summary>
/// Order entity that encapsulates a state machine for managing order lifecycle.
/// This demonstrates how to embed a state machine within a domain entity and expose
/// business methods that dispatch actions on the internal state machine.
/// </summary>
public class Order
{
    private readonly IEventBus _eventBus;
    private OrderStateMachine? _machine;
    
    public string Id { get; }
    public string CustomerId { get; }
    public List<string> Items { get; }
    public decimal TotalAmount { get; }
    public OrderState CurrentState => _machine?.CurrentOrderState ?? OrderState.Pending;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public string? ShippingAddress { get; private set; }
    public string? TrackingNumber { get; private set; }
    public string? CancellationReason { get; private set; }
    public string? ReturnReason { get; private set; }

    public Order(string id, string customerId, List<string> items, decimal totalAmount, IEventBus eventBus)
    {
        Id = id;
        CustomerId = customerId;
        Items = items ?? new List<string>();
        TotalAmount = totalAmount;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public async Task InitializeAsync()
    {
        _machine = await OrderStateMachine.CreateAsync(_eventBus);
        UpdatedAt = DateTime.UtcNow;
    }

    public async Task<bool> ConfirmAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Order not initialized");
        
        try
        {
            await _machine.DispatchAsync(OrderAction.Confirm);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> StartProcessingAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Order not initialized");
        
        try
        {
            await _machine.DispatchAsync(OrderAction.StartProcessing);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ShipAsync(string trackingNumber, string shippingAddress)
    {
        if (_machine == null) throw new InvalidOperationException("Order not initialized");
        
        try
        {
            await _machine.DispatchAsync(OrderAction.Ship);
            TrackingNumber = trackingNumber;
            ShippingAddress = shippingAddress;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeliverAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Order not initialized");
        
        try
        {
            await _machine.DispatchAsync(OrderAction.Deliver);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CancelAsync(string reason)
    {
        if (_machine == null) throw new InvalidOperationException("Order not initialized");
        
        try
        {
            await _machine.DispatchAsync(OrderAction.Cancel);
            CancellationReason = reason;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ReturnAsync(string reason)
    {
        if (_machine == null) throw new InvalidOperationException("Order not initialized");
        
        try
        {
            await _machine.DispatchAsync(OrderAction.Return);
            ReturnReason = reason;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
