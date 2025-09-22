---
layout: default
title: State Machine Pattern
nav_order: 3
parent: Patterns
permalink: /patterns/state-machine/
---

# State Machine Pattern

The State Machine pattern provides a structured way to handle complex state transitions and business logic. It's particularly useful for modeling workflows, processes, and entities with well-defined states and transitions.

## Overview

A state machine consists of:
- **States**: Represent the current condition of an entity
- **Transitions**: Define how to move from one state to another
- **Actions**: Operations that can be performed in each state
- **Guards**: Conditions that must be met for transitions to occur

## Benefits

- **Clear State Management**: Explicit state transitions with validation
- **Business Logic Encapsulation**: State-specific logic is contained
- **Error Handling**: Proper error states and recovery mechanisms
- **Auditability**: Complete state transition history
- **Testability**: Easy to test individual state transitions
- **Maintainability**: Clear separation of concerns

## Implementation with Magnett Automation Core

### 1. Define State Machine

```csharp
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Builders;

public static class OrderStateMachineDefinition
{
    public static IMachineDefinition Definition { get; } = MachineDefinitionBuilder.Create()
        .InitialState("Created")
        .AddState("Created")
            .OnAction("Validate").ToState("Validated")
            .OnAction("Cancel").ToState("Cancelled")
            .Build()
        .AddState("Validated")
            .OnAction("ReserveInventory").ToState("InventoryReserved")
            .OnAction("Cancel").ToState("Cancelled")
            .Build()
        .AddState("InventoryReserved")
            .OnAction("ProcessPayment").ToState("PaymentProcessing")
            .OnAction("ReleaseInventory").ToState("Validated")
            .OnAction("Cancel").ToState("Cancelled")
            .Build()
        .AddState("PaymentProcessing")
            .OnAction("PaymentSuccess").ToState("PaymentCompleted")
            .OnAction("PaymentFailed").ToState("PaymentFailed")
            .Build()
        .AddState("PaymentCompleted")
            .OnAction("Ship").ToState("Shipped")
            .OnAction("Refund").ToState("Refunded")
            .Build()
        .AddState("PaymentFailed")
            .OnAction("RetryPayment").ToState("PaymentProcessing")
            .OnAction("ReleaseInventory").ToState("Validated")
            .OnAction("Cancel").ToState("Cancelled")
            .Build()
        .AddState("Shipped")
            .OnAction("Deliver").ToState("Delivered")
            .OnAction("Return").ToState("Returned")
            .Build()
        .AddState("Delivered")
            .OnAction("Complete").ToState("Completed")
            .OnAction("Return").ToState("Returned")
            .Build()
        .AddState("Returned")
            .OnAction("ProcessRefund").ToState("Refunded")
            .Build()
        .AddState("Refunded")
            .Build()
        .AddState("Cancelled")
            .Build()
        .AddState("Completed")
            .Build()
        .Build();
}
```

### 2. Create Domain Entity

```csharp
public class Order
{
    public string Id { get; }
    public string CustomerId { get; }
    public decimal Amount { get; }
    public List<OrderItem> Items { get; }
    public string CurrentState { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public string PaymentTransactionId { get; private set; }
    public string ShippingTrackingNumber { get; private set; }
    public string ErrorMessage { get; private set; }
    public Dictionary<string, object> Metadata { get; }

    public Order(string id, string customerId, decimal amount, List<OrderItem> items)
    {
        Id = id;
        CustomerId = customerId;
        Amount = amount;
        Items = items;
        CurrentState = "Created";
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Metadata = new Dictionary<string, object>();
    }

    public void UpdateState(string newState, string transactionId = null, string trackingNumber = null, string errorMessage = null)
    {
        CurrentState = newState;
        UpdatedAt = DateTime.UtcNow;
        
        if (transactionId != null)
        {
            PaymentTransactionId = transactionId;
        }
        
        if (trackingNumber != null)
        {
            ShippingTrackingNumber = trackingNumber;
        }
        
        if (errorMessage != null)
        {
            ErrorMessage = errorMessage;
        }
    }

    public void SetMetadata(string key, object value)
    {
        Metadata[key] = value;
    }

    public T GetMetadata<T>(string key)
    {
        return Metadata.TryGetValue(key, out var value) ? (T)value : default(T);
    }
}

public class OrderItem
{
    public string ProductId { get; }
    public string ProductName { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }
    public decimal TotalPrice => Quantity * UnitPrice;

    public OrderItem(string productId, string productName, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
```

### 3. Create State Machine Service

```csharp
public class OrderStateMachineService
{
    private readonly IMachine _machine;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrderStateMachineService> _logger;
    private readonly IInventoryService _inventoryService;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IShippingService _shippingService;

    public OrderStateMachineService(
        IEventBus eventBus,
        ILogger<OrderStateMachineService> logger,
        IInventoryService inventoryService,
        IPaymentGateway paymentGateway,
        IShippingService shippingService)
    {
        _eventBus = eventBus;
        _logger = logger;
        _inventoryService = inventoryService;
        _paymentGateway = paymentGateway;
        _shippingService = shippingService;
    }

    private async Task<IMachine> GetMachineAsync()
    {
        if (_machine == null)
        {
            _machine = await Machine.CreateAsync(OrderStateMachineDefinition.Definition, _eventBus);
        }
        return _machine;
    }

    public async Task<bool> ValidateOrderAsync(Order order)
    {
        try
        {
            var machine = await GetMachineAsync();
            await machine.DispatchAsync("Validate");
            order.UpdateState("Validated");
            
            _logger.LogInformation("Order {OrderId} validated successfully", order.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating order {OrderId}", order.Id);
            return false;
        }
    }

    public async Task<bool> ReserveInventoryAsync(Order order)
    {
        try
        {
            var machine = await GetMachineAsync();
            await machine.DispatchAsync("ReserveInventory");
            order.UpdateState("InventoryReserved");
            
            // Reserve inventory for all items
            foreach (var item in order.Items)
            {
                var reserved = await _inventoryService.ReserveAsync(item.ProductId, item.Quantity);
                if (!reserved)
                {
                    _logger.LogWarning("Failed to reserve inventory for product {ProductId}", item.ProductId);
                    return false;
                }
            }
            
            _logger.LogInformation("Inventory reserved for order {OrderId}", order.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reserving inventory for order {OrderId}", order.Id);
            return false;
        }
    }

    public async Task<bool> ProcessPaymentAsync(Order order)
    {
        try
        {
            var machine = await GetMachineAsync();
            await machine.DispatchAsync("ProcessPayment");
            order.UpdateState("PaymentProcessing");
            
            var paymentResult = await _paymentGateway.ProcessPaymentAsync(order.Id, order.Amount);
            
            if (paymentResult.Success)
            {
                var machine = await GetMachineAsync();
            await machine.DispatchAsync("PaymentSuccess");
                order.UpdateState("PaymentCompleted", paymentResult.TransactionId);
                
                _logger.LogInformation("Payment processed successfully for order {OrderId}", order.Id);
                return true;
            }
            else
            {
                var machine = await GetMachineAsync();
            await machine.DispatchAsync("PaymentFailed");
                order.UpdateState("PaymentFailed", errorMessage: paymentResult.ErrorMessage);
                
                _logger.LogWarning("Payment failed for order {OrderId}: {Error}", order.Id, paymentResult.ErrorMessage);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for order {OrderId}", order.Id);
            
            var machine = await GetMachineAsync();
            await machine.DispatchAsync("PaymentFailed");
            order.UpdateState("PaymentFailed", errorMessage: ex.Message);
            
            return false;
        }
    }

    public async Task<bool> ShipOrderAsync(Order order)
    {
        try
        {
            var machine = await GetMachineAsync();
            await machine.DispatchAsync("Ship");
            order.UpdateState("Shipped");
            
            var trackingNumber = await _shippingService.CreateShipmentAsync(order.Id, order.Items);
            order.UpdateState("Shipped", trackingNumber: trackingNumber);
            
            _logger.LogInformation("Order {OrderId} shipped with tracking {TrackingNumber}", order.Id, trackingNumber);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error shipping order {OrderId}", order.Id);
            return false;
        }
    }

    public async Task<bool> DeliverOrderAsync(Order order)
    {
        try
        {
            var machine = await GetMachineAsync();
            await machine.DispatchAsync("Deliver");
            order.UpdateState("Delivered");
            
            _logger.LogInformation("Order {OrderId} delivered successfully", order.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error delivering order {OrderId}", order.Id);
            return false;
        }
    }

    public async Task<bool> CompleteOrderAsync(Order order)
    {
        try
        {
            var machine = await GetMachineAsync();
            await machine.DispatchAsync("Complete");
            order.UpdateState("Completed");
            
            _logger.LogInformation("Order {OrderId} completed successfully", order.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing order {OrderId}", order.Id);
            return false;
        }
    }

    public async Task<bool> CancelOrderAsync(Order order, string reason)
    {
        try
        {
            var machine = await GetMachineAsync();
            await machine.DispatchAsync("Cancel");
            order.UpdateState("Cancelled", errorMessage: reason);
            
            // Release inventory if reserved
            if (order.CurrentState == "InventoryReserved" || order.CurrentState == "PaymentCompleted")
            {
                foreach (var item in order.Items)
                {
                    await _inventoryService.ReleaseAsync(item.ProductId, item.Quantity);
                }
            }
            
            // Refund payment if completed
            if (order.CurrentState == "PaymentCompleted" || order.CurrentState == "Shipped" || order.CurrentState == "Delivered")
            {
                await _paymentGateway.RefundPaymentAsync(order.PaymentTransactionId);
            }
            
            _logger.LogInformation("Order {OrderId} cancelled: {Reason}", order.Id, reason);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order {OrderId}", order.Id);
            return false;
        }
    }

    public async Task<bool> RetryPaymentAsync(Order order)
    {
        try
        {
            var machine = await GetMachineAsync();
            await machine.DispatchAsync("RetryPayment");
            order.UpdateState("PaymentProcessing");
            
            var paymentResult = await _paymentGateway.ProcessPaymentAsync(order.Id, order.Amount);
            
            if (paymentResult.Success)
            {
                var machine = await GetMachineAsync();
            await machine.DispatchAsync("PaymentSuccess");
                order.UpdateState("PaymentCompleted", paymentResult.TransactionId);
                
                _logger.LogInformation("Payment retry successful for order {OrderId}", order.Id);
                return true;
            }
            else
            {
                var machine = await GetMachineAsync();
            await machine.DispatchAsync("PaymentFailed");
                order.UpdateState("PaymentFailed", errorMessage: paymentResult.ErrorMessage);
                
                _logger.LogWarning("Payment retry failed for order {OrderId}: {Error}", order.Id, paymentResult.ErrorMessage);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrying payment for order {OrderId}", order.Id);
            return false;
        }
    }
}
```

### 4. Create Service Interfaces

```csharp
public interface IInventoryService
{
    Task<bool> ReserveAsync(string productId, int quantity);
    Task ReleaseAsync(string productId, int quantity);
}

public interface IPaymentGateway
{
    Task<PaymentResult> ProcessPaymentAsync(string orderId, decimal amount);
    Task<RefundResult> RefundPaymentAsync(string transactionId);
}

public interface IShippingService
{
    Task<string> CreateShipmentAsync(string orderId, List<OrderItem> items);
}

public record PaymentResult
{
    public bool Success { get; init; }
    public string TransactionId { get; init; }
    public string ErrorMessage { get; init; }
}

public record RefundResult
{
    public bool Success { get; init; }
    public string TransactionId { get; init; }
    public decimal Amount { get; init; }
    public string ErrorMessage { get; init; }
}
```

### 5. Create Event Handlers

```csharp
public class OrderStateChangedHandler : IEventHandler<OrderStateChangedEvent>
{
    private readonly ILogger<OrderStateChangedHandler> _logger;

    public OrderStateChangedHandler(ILogger<OrderStateChangedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderStateChangedEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order {OrderId} state changed from {FromState} to {ToState}",
            @event.OrderId, @event.FromState, @event.ToState);
        
        // Here you could update read models, send notifications, etc.
        return Task.CompletedTask;
    }
}

public record OrderStateChangedEvent(string OrderId, string FromState, string ToState, DateTime Timestamp) 
    : Event("OrderStateChanged", "OrderService");
```

### 6. Usage Example

```csharp
public class OrderController
{
    private readonly OrderStateMachineService _orderService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(OrderStateMachineService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var orderId = Guid.NewGuid().ToString();
        var orderItems = request.Items.Select(i => new OrderItem(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)).ToList();
        var order = new Order(orderId, request.CustomerId, request.Amount, orderItems);
        
        return Ok(new { OrderId = orderId, State = order.CurrentState });
    }

    [HttpPost("orders/{orderId}/validate")]
    public async Task<IActionResult> ValidateOrder(string orderId)
    {
        var order = await GetOrderAsync(orderId);
        if (order == null) return NotFound();
        
        var success = await _orderService.ValidateOrderAsync(order);
        
        return success ? Ok(new { State = order.CurrentState }) : BadRequest("Validation failed");
    }

    [HttpPost("orders/{orderId}/reserve-inventory")]
    public async Task<IActionResult> ReserveInventory(string orderId)
    {
        var order = await GetOrderAsync(orderId);
        if (order == null) return NotFound();
        
        var success = await _orderService.ReserveInventoryAsync(order);
        
        return success ? Ok(new { State = order.CurrentState }) : BadRequest("Inventory reservation failed");
    }

    [HttpPost("orders/{orderId}/process-payment")]
    public async Task<IActionResult> ProcessPayment(string orderId)
    {
        var order = await GetOrderAsync(orderId);
        if (order == null) return NotFound();
        
        var success = await _orderService.ProcessPaymentAsync(order);
        
        return success ? Ok(new { State = order.CurrentState, TransactionId = order.PaymentTransactionId }) : BadRequest("Payment processing failed");
    }

    [HttpPost("orders/{orderId}/ship")]
    public async Task<IActionResult> ShipOrder(string orderId)
    {
        var order = await GetOrderAsync(orderId);
        if (order == null) return NotFound();
        
        var success = await _orderService.ShipOrderAsync(order);
        
        return success ? Ok(new { State = order.CurrentState, TrackingNumber = order.ShippingTrackingNumber }) : BadRequest("Shipping failed");
    }

    [HttpPost("orders/{orderId}/deliver")]
    public async Task<IActionResult> DeliverOrder(string orderId)
    {
        var order = await GetOrderAsync(orderId);
        if (order == null) return NotFound();
        
        var success = await _orderService.DeliverOrderAsync(order);
        
        return success ? Ok(new { State = order.CurrentState }) : BadRequest("Delivery failed");
    }

    [HttpPost("orders/{orderId}/complete")]
    public async Task<IActionResult> CompleteOrder(string orderId)
    {
        var order = await GetOrderAsync(orderId);
        if (order == null) return NotFound();
        
        var success = await _orderService.CompleteOrderAsync(order);
        
        return success ? Ok(new { State = order.CurrentState }) : BadRequest("Completion failed");
    }

    [HttpPost("orders/{orderId}/cancel")]
    public async Task<IActionResult> CancelOrder(string orderId, [FromBody] CancelOrderRequest request)
    {
        var order = await GetOrderAsync(orderId);
        if (order == null) return NotFound();
        
        var success = await _orderService.CancelOrderAsync(order, request.Reason);
        
        return success ? Ok(new { State = order.CurrentState }) : BadRequest("Cancellation failed");
    }

    [HttpPost("orders/{orderId}/retry-payment")]
    public async Task<IActionResult> RetryPayment(string orderId)
    {
        var order = await GetOrderAsync(orderId);
        if (order == null) return NotFound();
        
        var success = await _orderService.RetryPaymentAsync(order);
        
        return success ? Ok(new { State = order.CurrentState }) : BadRequest("Payment retry failed");
    }

    private async Task<Order> GetOrderAsync(string orderId)
    {
        // In a real application, this would query a repository
        // For this example, we'll return a mock order
        return new Order(orderId, "customer-123", 100.00m, new List<OrderItem>());
    }
}
```

## Advanced Patterns

### Hierarchical State Machines

```csharp
public static class HierarchicalStateMachineDefinition
{
    public static IMachineDefinition Definition { get; } = MachineDefinitionBuilder.Create()
        .WithInitialState("Active")
        .AddState("Active")
            .OnAction("Start").ToState("Processing")
            .OnAction("Pause").ToState("Paused")
            .Build()
        .AddState("Processing")
            .OnAction("Complete").ToState("Completed")
            .OnAction("Error").ToState("Error")
            .OnAction("Pause").ToState("Paused")
            .Build()
        .AddState("Paused")
            .OnAction("Resume").ToState("Active")
            .OnAction("Stop").ToState("Stopped")
            .Build()
        .AddState("Error")
            .OnAction("Retry").ToState("Active")
            .OnAction("Stop").ToState("Stopped")
            .Build()
        .AddState("Completed")
            .Build()
        .AddState("Stopped")
            .Build()
        .Build();
}
```

### State Machine with Guards

```csharp
public class OrderStateMachineWithGuards
{
    private readonly IMachine _machine;
    private readonly IEventBus _eventBus;

    public OrderStateMachineWithGuards(IEventBus eventBus)
    {
        _eventBus = eventBus;
        _machine = await Machine.CreateAsync(OrderStateMachineDefinition.Definition, eventBus);
    }

    public async Task<bool> CanTransitionTo(string targetState, Order order)
    {
        // Implement guard logic
        switch (targetState)
        {
            case "Validated":
                return order.CurrentState == "Created" && order.Items.Any();
            case "InventoryReserved":
                return order.CurrentState == "Validated" && await HasInventoryAvailable(order);
            case "PaymentProcessing":
                return order.CurrentState == "InventoryReserved" && order.Amount > 0;
            default:
                return false;
        }
    }

    private async Task<bool> HasInventoryAvailable(Order order)
    {
        // Check inventory availability
        return true; // Simplified for example
    }
}
```

## Best Practices

1. **State Design**: Keep states focused and meaningful
2. **Action Names**: Use clear, descriptive action names
3. **Error States**: Include proper error handling states
4. **Validation**: Validate state transitions before executing
5. **Logging**: Log all state transitions for debugging
6. **Testing**: Test all possible state transitions
7. **Documentation**: Document state machine behavior clearly
8. **Guards**: Use guards to prevent invalid transitions

## When to Use State Machines

- **Complex Workflows**: When you have complex business processes
- **State Validation**: When you need to validate state transitions
- **Audit Requirements**: When you need to track state changes
- **Error Handling**: When you need robust error handling
- **Business Rules**: When business rules depend on current state
- **Integration**: When integrating with external systems that have states
