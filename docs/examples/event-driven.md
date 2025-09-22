---
layout: default
title: Event-Driven Architecture Example
nav_order: 2
parent: Examples
permalink: /examples/event-driven/
---

# Event-Driven Architecture Example

This example demonstrates how to build a complete event-driven system using Magnett Automation Core, including event handling, metrics collection, and error handling.

## Overview

We'll build a simple e-commerce system that processes orders through events. The system will include:

- Order creation and processing
- Inventory management
- Payment processing
- Notification system
- Metrics and monitoring

## Implementation

### 1. Define Domain Events

```csharp
using Magnett.Automation.Core.Events;

// Order events
public record OrderCreatedEvent(string OrderId, string CustomerId, decimal Amount, List<OrderItem> Items) 
    : Event("OrderCreated", "OrderService");

public record OrderValidatedEvent(string OrderId, bool IsValid, string Reason) 
    : Event("OrderValidated", "OrderService");

public record OrderProcessingStartedEvent(string OrderId) 
    : Event("OrderProcessingStarted", "OrderService");

public record OrderCompletedEvent(string OrderId, decimal FinalAmount) 
    : Event("OrderCompleted", "OrderService");

public record OrderFailedEvent(string OrderId, string Reason) 
    : Event("OrderFailed", "OrderService");

// Inventory events
public record InventoryReservedEvent(string OrderId, string ProductId, int Quantity) 
    : Event("InventoryReserved", "InventoryService");

public record InventoryReleasedEvent(string OrderId, string ProductId, int Quantity) 
    : Event("InventoryReleased", "InventoryService");

public record InventoryInsufficientEvent(string OrderId, string ProductId, int Requested, int Available) 
    : Event("InventoryInsufficient", "InventoryService");

// Payment events
public record PaymentInitiatedEvent(string OrderId, decimal Amount, string PaymentMethod) 
    : Event("PaymentInitiated", "PaymentService");

public record PaymentCompletedEvent(string OrderId, string TransactionId, decimal Amount) 
    : Event("PaymentCompleted", "PaymentService");

public record PaymentFailedEvent(string OrderId, string Reason) 
    : Event("PaymentFailed", "PaymentService");

// Notification events
public record OrderConfirmationSentEvent(string OrderId, string CustomerEmail) 
    : Event("OrderConfirmationSent", "NotificationService");

public record OrderFailureNotificationSentEvent(string OrderId, string CustomerEmail, string Reason) 
    : Event("OrderFailureNotificationSent", "NotificationService");

public record OrderItem
{
    public string ProductId { get; init; }
    public string ProductName { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice => Quantity * UnitPrice;
}
```

### 2. Create Event Handlers

```csharp
using Magnett.Automation.Core.Events;
using Microsoft.Extensions.Logging;

public class OrderValidationHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly IInventoryService _inventoryService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrderValidationHandler> _logger;

    public OrderValidationHandler(
        IInventoryService inventoryService, 
        IEventBus eventBus, 
        ILogger<OrderValidationHandler> logger)
    {
        _inventoryService = inventoryService;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Validating order {OrderId}", @event.OrderId);

        try
        {
            // Check inventory for all items
            var validationResults = new List<(string ProductId, bool IsValid, string Reason)>();
            
            foreach (var item in @event.Items)
            {
                var available = await _inventoryService.GetAvailableQuantityAsync(item.ProductId);
                
                if (available >= item.Quantity)
                {
                    validationResults.Add((item.ProductId, true, "Sufficient inventory"));
                }
                else
                {
                    validationResults.Add((item.ProductId, false, $"Insufficient inventory. Available: {available}, Requested: {item.Quantity}"));
                }
            }

            var isValid = validationResults.All(r => r.IsValid);
            var reason = isValid ? "Order validated successfully" : string.Join("; ", validationResults.Where(r => !r.IsValid).Select(r => r.Reason));

            var validationEvent = new OrderValidatedEvent(@event.OrderId, isValid, reason);
            await _eventBus.PublishAsync(validationEvent);

            _logger.LogInformation("Order {OrderId} validation completed. Valid: {IsValid}", @event.OrderId, isValid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating order {OrderId}", @event.OrderId);
            
            var validationEvent = new OrderValidatedEvent(@event.OrderId, false, $"Validation error: {ex.Message}");
            await _eventBus.PublishAsync(validationEvent);
        }
    }
}

public class InventoryReservationHandler : IEventHandler<OrderValidatedEvent>
{
    private readonly IInventoryService _inventoryService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<InventoryReservationHandler> _logger;

    public InventoryReservationHandler(
        IInventoryService inventoryService, 
        IEventBus eventBus, 
        ILogger<InventoryReservationHandler> logger)
    {
        _inventoryService = inventoryService;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(OrderValidatedEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        if (!@event.IsValid)
        {
            _logger.LogInformation("Skipping inventory reservation for invalid order {OrderId}", @event.OrderId);
            return;
        }

        _logger.LogInformation("Reserving inventory for order {OrderId}", @event.OrderId);

        try
        {
            // Get order details (in a real system, this would come from a repository)
            var order = await GetOrderDetailsAsync(@event.OrderId);
            
            foreach (var item in order.Items)
            {
                var reserved = await _inventoryService.ReserveAsync(item.ProductId, item.Quantity);
                
                if (reserved)
                {
                    var reservedEvent = new InventoryReservedEvent(@event.OrderId, item.ProductId, item.Quantity);
                    await _eventBus.PublishAsync(reservedEvent);
                }
                else
                {
                    var insufficientEvent = new InventoryInsufficientEvent(@event.OrderId, item.ProductId, item.Quantity, 0);
                    await _eventBus.PublishAsync(insufficientEvent);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reserving inventory for order {OrderId}", @event.OrderId);
        }
    }

    private async Task<OrderCreatedEvent> GetOrderDetailsAsync(string orderId)
    {
        // In a real system, this would query a repository
        // For this example, we'll return a mock order
        return new OrderCreatedEvent(orderId, "customer-123", 100.00m, new List<OrderItem>
        {
            new OrderItem { ProductId = "product-1", ProductName = "Widget", Quantity = 2, UnitPrice = 50.00m }
        });
    }
}

public class PaymentProcessingHandler : IEventHandler<InventoryReservedEvent>
{
    private readonly IPaymentService _paymentService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<PaymentProcessingHandler> _logger;

    public PaymentProcessingHandler(
        IPaymentService paymentService, 
        IEventBus eventBus, 
        ILogger<PaymentProcessingHandler> logger)
    {
        _paymentService = paymentService;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(InventoryReservedEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing payment for order {OrderId}", @event.OrderId);

        try
        {
            // Get order details
            var order = await GetOrderDetailsAsync(@event.OrderId);
            
            var paymentInitiatedEvent = new PaymentInitiatedEvent(@event.OrderId, order.Amount, "CreditCard");
            await _eventBus.PublishAsync(paymentInitiatedEvent);

            // Process payment
            var paymentResult = await _paymentService.ProcessPaymentAsync(@event.OrderId, order.Amount);
            
            if (paymentResult.Success)
            {
                var paymentCompletedEvent = new PaymentCompletedEvent(@event.OrderId, paymentResult.TransactionId, order.Amount);
                await _eventBus.PublishAsync(paymentCompletedEvent);
            }
            else
            {
                var paymentFailedEvent = new PaymentFailedEvent(@event.OrderId, paymentResult.ErrorMessage);
                await _eventBus.PublishAsync(paymentFailedEvent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for order {OrderId}", @event.OrderId);
            
            var paymentFailedEvent = new PaymentFailedEvent(@event.OrderId, $"Payment processing error: {ex.Message}");
            await _eventBus.PublishAsync(paymentFailedEvent);
        }
    }

    private async Task<OrderCreatedEvent> GetOrderDetailsAsync(string orderId)
    {
        // Mock implementation
        return new OrderCreatedEvent(orderId, "customer-123", 100.00m, new List<OrderItem>());
    }
}

public class NotificationHandler : IEventHandler<OrderCompletedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationHandler> _logger;

    public NotificationHandler(
        INotificationService notificationService, 
        ILogger<NotificationHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(OrderCompletedEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending confirmation notification for order {OrderId}", @event.OrderId);

        try
        {
            // Get customer email (in a real system, this would come from a repository)
            var customerEmail = await GetCustomerEmailAsync(@event.OrderId);
            
            await _notificationService.SendOrderConfirmationAsync(customerEmail, @event.OrderId, @event.FinalAmount);
            
            var confirmationSentEvent = new OrderConfirmationSentEvent(@event.OrderId, customerEmail);
            // Note: In a real system, you'd publish this event to the event bus
            
            _logger.LogInformation("Confirmation notification sent for order {OrderId}", @event.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification for order {OrderId}", @event.OrderId);
        }
    }

    private async Task<string> GetCustomerEmailAsync(string orderId)
    {
        // Mock implementation
        return "customer@example.com";
    }
}
```

### 3. Create Service Interfaces

```csharp
public interface IInventoryService
{
    Task<int> GetAvailableQuantityAsync(string productId);
    Task<bool> ReserveAsync(string productId, int quantity);
    Task ReleaseAsync(string productId, int quantity);
}

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(string orderId, decimal amount);
}

public interface INotificationService
{
    Task SendOrderConfirmationAsync(string email, string orderId, decimal amount);
    Task SendOrderFailureNotificationAsync(string email, string orderId, string reason);
}

public record PaymentResult
{
    public bool Success { get; init; }
    public string TransactionId { get; init; }
    public string ErrorMessage { get; init; }
}
```

### 4. Create Service Implementations

```csharp
public class InventoryService : IInventoryService
{
    private readonly Dictionary<string, int> _inventory = new()
    {
        { "product-1", 100 },
        { "product-2", 50 },
        { "product-3", 25 }
    };

    private readonly Dictionary<string, int> _reserved = new();

    public Task<int> GetAvailableQuantityAsync(string productId)
    {
        var total = _inventory.GetValueOrDefault(productId, 0);
        var reserved = _reserved.GetValueOrDefault(productId, 0);
        return Task.FromResult(total - reserved);
    }

    public Task<bool> ReserveAsync(string productId, int quantity)
    {
        var available = GetAvailableQuantityAsync(productId).Result;
        
        if (available >= quantity)
        {
            _reserved[productId] = _reserved.GetValueOrDefault(productId, 0) + quantity;
            return Task.FromResult(true);
        }
        
        return Task.FromResult(false);
    }

    public Task ReleaseAsync(string productId, int quantity)
    {
        if (_reserved.ContainsKey(productId))
        {
            _reserved[productId] = Math.Max(0, _reserved[productId] - quantity);
        }
        return Task.CompletedTask;
    }
}

public class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly Random _random = new();

    public PaymentService(ILogger<PaymentService> logger)
    {
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(string orderId, decimal amount)
    {
        _logger.LogInformation("Processing payment for order {OrderId}, amount: {Amount}", orderId, amount);
        
        // Simulate payment processing delay
        await Task.Delay(1000);
        
        // Simulate random payment failures (10% failure rate)
        if (_random.NextDouble() < 0.1)
        {
            return new PaymentResult
            {
                Success = false,
                ErrorMessage = "Payment declined by bank"
            };
        }
        
        var transactionId = Guid.NewGuid().ToString();
        
        _logger.LogInformation("Payment successful for order {OrderId}, transaction: {TransactionId}", orderId, transactionId);
        
        return new PaymentResult
        {
            Success = true,
            TransactionId = transactionId
        };
    }
}

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendOrderConfirmationAsync(string email, string orderId, decimal amount)
    {
        _logger.LogInformation("Sending order confirmation to {Email} for order {OrderId}", email, orderId);
        
        // Simulate email sending
        await Task.Delay(500);
        
        _logger.LogInformation("Order confirmation sent to {Email}", email);
    }

    public async Task SendOrderFailureNotificationAsync(string email, string orderId, string reason)
    {
        _logger.LogInformation("Sending order failure notification to {Email} for order {OrderId}", email, orderId);
        
        // Simulate email sending
        await Task.Delay(500);
        
        _logger.LogInformation("Order failure notification sent to {Email}", email);
    }
}
```

### 5. Create Application Service

```csharp
public class OrderApplicationService
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrderApplicationService> _logger;

    public OrderApplicationService(IEventBus eventBus, ILogger<OrderApplicationService> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<string> CreateOrderAsync(string customerId, List<OrderItem> items)
    {
        var orderId = Guid.NewGuid().ToString();
        var totalAmount = items.Sum(item => item.TotalPrice);
        
        var orderCreatedEvent = new OrderCreatedEvent(orderId, customerId, totalAmount, items);
        await _eventBus.PublishAsync(orderCreatedEvent);
        
        _logger.LogInformation("Order {OrderId} created for customer {CustomerId}", orderId, customerId);
        
        return orderId;
    }
}
```

### 6. Setup and Usage

```csharp
public class Program
{
    public static async Task Main(string[] args)
    {
        // Setup logging
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        
        // Create event bus
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        
        // Register event handlers
        eventBus.EventHandlerRegistry.Register<OrderValidationHandler>();
        eventBus.EventHandlerRegistry.Register<InventoryReservationHandler>();
        eventBus.EventHandlerRegistry.Register<PaymentProcessingHandler>();
        eventBus.EventHandlerRegistry.Register<NotificationHandler>();
        
        // Start event bus
        eventBus.Start();
        
        // Create services
        var inventoryService = new InventoryService();
        var paymentService = new PaymentService(loggerFactory.CreateLogger<PaymentService>());
        var notificationService = new NotificationService(loggerFactory.CreateLogger<NotificationService>());
        
        // Create application service
        var orderService = new OrderApplicationService(eventBus, loggerFactory.CreateLogger<OrderApplicationService>());
        
        // Create sample order
        var items = new List<OrderItem>
        {
            new OrderItem { ProductId = "product-1", ProductName = "Widget", Quantity = 2, UnitPrice = 50.00m },
            new OrderItem { ProductId = "product-2", ProductName = "Gadget", Quantity = 1, UnitPrice = 75.00m }
        };
        
        var orderId = await orderService.CreateOrderAsync("customer-123", items);
        
        Console.WriteLine($"Order {orderId} created. Processing will happen asynchronously through events.");
        
        // Wait for processing to complete
        await Task.Delay(5000);
        
        // Stop event bus
        await eventBus.StopAsync();
    }
}
```

## Key Benefits

1. **Loose Coupling**: Services communicate through events, not direct calls
2. **Scalability**: Each service can be scaled independently
3. **Resilience**: If one service fails, others continue to work
4. **Extensibility**: Easy to add new services that react to events
5. **Auditability**: Complete event trail for debugging and compliance
6. **Asynchronous Processing**: Non-blocking operations improve performance

## View Complete Example

The fully functional example is available at:
- Source code: `test/Magnett.Automation.Core.IntegrationTest/Events/`
- Tests: `test/Magnett.Automation.Core.IntegrationTest/Events/EventTest.cs`

## Run Example

```bash
# Run all Event-Driven Architecture tests
dotnet test test/Magnett.Automation.Core.IntegrationTest/Magnett.Automation.Core.IntegrationTest.csproj --filter "EventTest" --verbosity minimal

# Run specific test
dotnet test test/Magnett.Automation.Core.IntegrationTest/Magnett.Automation.Core.IntegrationTest.csproj --filter "EventTest_WhenPublishEvent_ShouldBeHandled" --verbosity minimal
```

## Example Structure

```
Events/
├── Events/
│   └── TestEvent.cs
├── EventHandlers/
│   ├── ConsoleEventHandler.cs
│   └── ConsoleWarningEventHandler.cs
└── EventTest.cs
```

## Best Practices

1. **Event Design**: Make events immutable and include all necessary context
2. **Error Handling**: Implement proper error handling in event handlers
3. **Idempotency**: Ensure event handlers are idempotent
4. **Monitoring**: Implement comprehensive logging and metrics
5. **Testing**: Test event handlers in isolation
6. **Documentation**: Document event contracts and dependencies
