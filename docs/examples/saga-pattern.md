---
layout: default
title: Saga Pattern Example
nav_order: 1
parent: Examples
permalink: /examples/saga-pattern/
---

# Saga Pattern Example

This example demonstrates a complete implementation of the Saga pattern for distributed transactions with automatic compensation using Magnett Automation Core.

## Overview

The Saga pattern manages distributed transactions by breaking them into a series of local transactions, each with a corresponding compensation transaction. This example shows an e-commerce order processing system.

## Implementation

### 1. Define Domain Events

```csharp
using Magnett.Automation.Core.Events;

// Order events
public record OrderCreatedEvent(string OrderId, string CustomerId, decimal Amount, List<OrderItem> Items) 
    : Event("OrderCreated", "OrderService");

public record OrderValidatedEvent(string OrderId, bool IsValid, string Reason) 
    : Event("OrderValidated", "OrderService");

public record InventoryReservedEvent(string OrderId, string ProductId, int Quantity) 
    : Event("InventoryReserved", "InventoryService");

public record PaymentProcessedEvent(string OrderId, string TransactionId, decimal Amount) 
    : Event("PaymentProcessed", "PaymentService");

public record OrderCompletedEvent(string OrderId, decimal FinalAmount) 
    : Event("OrderCompleted", "OrderService");

public record OrderFailedEvent(string OrderId, string Reason) 
    : Event("OrderFailed", "OrderService");

// Compensation events
public record InventoryReleasedEvent(string OrderId, string ProductId, int Quantity) 
    : Event("InventoryReleased", "InventoryService");

public record PaymentRefundedEvent(string OrderId, string TransactionId, decimal Amount) 
    : Event("PaymentRefunded", "PaymentService");

public record OrderCancelledEvent(string OrderId, string Reason) 
    : Event("OrderCancelled", "OrderService");

public record OrderItem
{
    public string ProductId { get; init; }
    public string ProductName { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice => Quantity * UnitPrice;
}
```

### 2. Create Saga Workflow Nodes

```csharp
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;
using Magnett.Automation.Core.WorkFlows.Runtimes;

public class CreateOrderNode : NodeAsync
{
    private readonly ContextField<string> _orderIdField = ContextField<string>.WithName("OrderId");
    private readonly ContextField<string> _customerIdField = ContextField<string>.WithName("CustomerId");
    private readonly ContextField<decimal> _amountField = ContextField<decimal>.WithName("Amount");
    private readonly ContextField<List<OrderItem>> _itemsField = ContextField<List<OrderItem>>.WithName("Items");

    public CreateOrderNode(CommonNamedKey name, IEventBus eventBus) : base(name, eventBus)
    {
    }

    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return NodeExit.Cancelled(ExitCode.Cancelled, "Operation cancelled");
        }

        var orderId = Guid.NewGuid().ToString();
        var customerId = context.Value(_customerIdField);
        var amount = context.Value(_amountField);
        var items = context.Value(_itemsField);

        // Store order data in context
        await context.StoreAsync(_orderIdField, orderId);

        // Publish order created event
        await EventBus.PublishAsync(new OrderCreatedEvent(orderId, customerId, amount, items));

        return NodeExit.Completed(ExitCode.Created, $"Order {orderId} created successfully");
    }
}

public class ValidateOrderNode : NodeAsync
{
    private readonly ContextField<string> _orderIdField = ContextField<string>.WithName("OrderId");
    private readonly ContextField<List<OrderItem>> _itemsField = ContextField<List<OrderItem>>.WithName("Items");
    private readonly IInventoryService _inventoryService;

    public ValidateOrderNode(CommonNamedKey name, IEventBus eventBus, IInventoryService inventoryService) 
        : base(name, eventBus)
    {
        _inventoryService = inventoryService;
    }

    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return NodeExit.Cancelled(ExitCode.Cancelled, "Operation cancelled");
        }

        var orderId = context.Value(_orderIdField);
        var items = context.Value(_itemsField);

        // Validate inventory for all items
        foreach (var item in items)
        {
            var available = await _inventoryService.GetAvailableQuantityAsync(item.ProductId);
            if (available < item.Quantity)
            {
                await EventBus.PublishAsync(new OrderValidatedEvent(orderId, false, 
                    $"Insufficient inventory for {item.ProductName}. Available: {available}, Required: {item.Quantity}"));
                
                return NodeExit.Failed(ExitCode.Denied, $"Order validation failed: insufficient inventory");
            }
        }

        await EventBus.PublishAsync(new OrderValidatedEvent(orderId, true, "Order validated successfully"));
        
        return NodeExit.Completed(ExitCode.Done, "Order validated successfully");
    }
}

public class ReserveInventoryNode : NodeAsync
{
    private readonly ContextField<string> _orderIdField = ContextField<string>.WithName("OrderId");
    private readonly ContextField<List<OrderItem>> _itemsField = ContextField<List<OrderItem>>.WithName("Items");
    private readonly IInventoryService _inventoryService;

    public ReserveInventoryNode(CommonNamedKey name, IEventBus eventBus, IInventoryService inventoryService) 
        : base(name, eventBus)
    {
        _inventoryService = inventoryService;
    }

    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return NodeExit.Cancelled(ExitCode.Cancelled, "Operation cancelled");
        }

        var orderId = context.Value(_orderIdField);
        var items = context.Value(_itemsField);

        // Reserve inventory for all items
        foreach (var item in items)
        {
            var reserved = await _inventoryService.ReserveAsync(item.ProductId, item.Quantity);
            if (!reserved)
            {
                // Compensation: Release previously reserved items
                await CompensateReservations(context, items.TakeWhile(i => i != item).ToList());
                
                return NodeExit.Failed(ExitCode.Denied, $"Failed to reserve inventory for {item.ProductName}");
            }

            await EventBus.PublishAsync(new InventoryReservedEvent(orderId, item.ProductId, item.Quantity));
        }

        return NodeExit.Completed(ExitCode.Done, "Inventory reserved successfully");
    }

    private async Task CompensateReservations(Context context, List<OrderItem> itemsToRelease)
    {
        var orderId = context.Value(_orderIdField);
        
        foreach (var item in itemsToRelease)
        {
            await _inventoryService.ReleaseAsync(item.ProductId, item.Quantity);
            await EventBus.PublishAsync(new InventoryReleasedEvent(orderId, item.ProductId, item.Quantity));
        }
    }
}

public class ProcessPaymentNode : NodeAsync
{
    private readonly ContextField<string> _orderIdField = ContextField<string>.WithName("OrderId");
    private readonly ContextField<decimal> _amountField = ContextField<decimal>.WithName("Amount");
    private readonly IPaymentService _paymentService;

    public ProcessPaymentNode(CommonNamedKey name, IEventBus eventBus, IPaymentService paymentService) 
        : base(name, eventBus)
    {
        _paymentService = paymentService;
    }

    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return NodeExit.Cancelled(ExitCode.Cancelled, "Operation cancelled");
        }

        var orderId = context.Value(_orderIdField);
        var amount = context.Value(_amountField);

        var paymentResult = await _paymentService.ProcessPaymentAsync(orderId, amount);
        
        if (paymentResult.Success)
        {
            await EventBus.PublishAsync(new PaymentProcessedEvent(orderId, paymentResult.TransactionId, amount));
            return NodeExit.Completed(ExitCode.Done, $"Payment processed successfully: {paymentResult.TransactionId}");
        }
        else
        {
            return NodeExit.Failed(ExitCode.Denied, $"Payment failed: {paymentResult.ErrorMessage}");
        }
    }
}

public class CompleteOrderNode : NodeAsync
{
    private readonly ContextField<string> _orderIdField = ContextField<string>.WithName("OrderId");
    private readonly ContextField<decimal> _amountField = ContextField<decimal>.WithName("Amount");

    public CompleteOrderNode(CommonNamedKey name, IEventBus eventBus) : base(name, eventBus)
    {
    }

    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return NodeExit.Cancelled(ExitCode.Cancelled, "Operation cancelled");
        }

        var orderId = context.Value(_orderIdField);
        var amount = context.Value(_amountField);

        await EventBus.PublishAsync(new OrderCompletedEvent(orderId, amount));
        
        return NodeExit.Completed(ExitCode.Done, $"Order {orderId} completed successfully");
    }
}
```

### 3. Create Compensation Nodes

```csharp
public class CancelOrderNode : NodeAsync
{
    private readonly ContextField<string> _orderIdField = ContextField<string>.WithName("OrderId");
    private readonly ContextField<List<OrderItem>> _itemsField = ContextField<List<OrderItem>>.WithName("Items");
    private readonly IInventoryService _inventoryService;
    private readonly IPaymentService _paymentService;

    public CancelOrderNode(CommonNamedKey name, IEventBus eventBus, IInventoryService inventoryService, IPaymentService paymentService) 
        : base(name, eventBus)
    {
        _inventoryService = inventoryService;
        _paymentService = paymentService;
    }

    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return NodeExit.Cancelled(ExitCode.Cancelled, "Operation cancelled");
        }

        var orderId = context.Value(_orderIdField);
        var items = context.Value(_itemsField);

        // Release inventory
        foreach (var item in items)
        {
            await _inventoryService.ReleaseAsync(item.ProductId, item.Quantity);
            await EventBus.PublishAsync(new InventoryReleasedEvent(orderId, item.ProductId, item.Quantity));
        }

        // Refund payment if processed
        var refundResult = await _paymentService.RefundPaymentAsync(orderId);
        if (refundResult.Success)
        {
            await EventBus.PublishAsync(new PaymentRefundedEvent(orderId, refundResult.TransactionId, refundResult.Amount));
        }

        await EventBus.PublishAsync(new OrderCancelledEvent(orderId, "Order cancelled due to processing failure"));
        
        return NodeExit.Completed(ExitCode.Cancelled, $"Order {orderId} cancelled and compensated");
    }
}
```

### 4. Create Saga Workflow Definition

```csharp
using Magnett.Automation.Core.WorkFlows.Definitions.Builders;

public static class OrderSagaDefinition
{
    public static IFlowDefinition CreateDefinition()
    {
        return FlowDefinitionBuilder.Create()
            .WithInitialNode<CreateOrderNode>(NodeName.CreateOrder)
                .OnExitCode(ExitCode.Created).GoTo(NodeName.ValidateOrder)
                .OnExitCode(ExitCode.Cancelled).GoTo(NodeName.CancelOrder)
                .Build()
            .WithNode<ValidateOrderNode>(NodeName.ValidateOrder)
                .OnExitCode(ExitCode.Done).GoTo(NodeName.ReserveInventory)
                .OnExitCode(ExitCode.Denied).GoTo(NodeName.CancelOrder)
                .OnExitCode(ExitCode.Cancelled).GoTo(NodeName.CancelOrder)
                .Build()
            .WithNode<ReserveInventoryNode>(NodeName.ReserveInventory)
                .OnExitCode(ExitCode.Done).GoTo(NodeName.ProcessPayment)
                .OnExitCode(ExitCode.Denied).GoTo(NodeName.CancelOrder)
                .OnExitCode(ExitCode.Cancelled).GoTo(NodeName.CancelOrder)
                .Build()
            .WithNode<ProcessPaymentNode>(NodeName.ProcessPayment)
                .OnExitCode(ExitCode.Done).GoTo(NodeName.CompleteOrder)
                .OnExitCode(ExitCode.Denied).GoTo(NodeName.CancelOrder)
                .OnExitCode(ExitCode.Cancelled).GoTo(NodeName.CancelOrder)
                .Build()
            .WithNode<CompleteOrderNode>(NodeName.CompleteOrder)
                .OnExitCode(ExitCode.Done).GoTo(NodeName.End)
                .OnExitCode(ExitCode.Cancelled).GoTo(NodeName.CancelOrder)
                .Build()
            .WithNode<CancelOrderNode>(NodeName.CancelOrder)
                .OnExitCode(ExitCode.Cancelled).GoTo(NodeName.End)
                .Build()
            .WithNode<EndNode>(NodeName.End)
                .Build()
            .BuildDefinition();
    }

    public static class NodeName
    {
        public static readonly CommonNamedKey CreateOrder = CommonNamedKey.WithName("CreateOrder");
        public static readonly CommonNamedKey ValidateOrder = CommonNamedKey.WithName("ValidateOrder");
        public static readonly CommonNamedKey ReserveInventory = CommonNamedKey.WithName("ReserveInventory");
        public static readonly CommonNamedKey ProcessPayment = CommonNamedKey.WithName("ProcessPayment");
        public static readonly CommonNamedKey CompleteOrder = CommonNamedKey.WithName("CompleteOrder");
        public static readonly CommonNamedKey CancelOrder = CommonNamedKey.WithName("CancelOrder");
        public static readonly CommonNamedKey End = CommonNamedKey.WithName("End");
    }
}
```

### 5. Create Service Interfaces

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
    Task<RefundResult> RefundPaymentAsync(string orderId);
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

### 6. Usage Example

```csharp
public class OrderSagaService
{
    private readonly IFlowRunner _flowRunner;
    private readonly IEventBus _eventBus;

    public OrderSagaService(IFlowRunner flowRunner, IEventBus eventBus)
    {
        _flowRunner = flowRunner;
        _eventBus = eventBus;
    }

    public async Task<string> ProcessOrderAsync(string customerId, decimal amount, List<OrderItem> items)
    {
        // Create context with order data
        var context = new Context();
        await context.StoreAsync(ContextField<string>.WithName("CustomerId"), customerId);
        await context.StoreAsync(ContextField<decimal>.WithName("Amount"), amount);
        await context.StoreAsync(ContextField<List<OrderItem>>.WithName("Items"), items);

        // Create flow definition
        var flowDefinition = OrderSagaDefinition.CreateDefinition();

        // Run the saga
        var result = await _flowRunner.RunAsync(flowDefinition, context);

        // Get order ID from context
        var orderId = context.Value<string>("OrderId");

        if (result.State == ExitState.Completed)
        {
            Console.WriteLine($"Order {orderId} processed successfully");
        }
        else
        {
            Console.WriteLine($"Order {orderId} failed: {result.Message}");
        }

        return orderId;
    }
}
```

## Key Benefits

1. **Automatic Compensation**: Failed transactions are automatically compensated
2. **Event-Driven**: All steps are communicated through events
3. **Resilient**: Handles failures gracefully with rollback
4. **Auditable**: Complete event trail for debugging
5. **Scalable**: Each step can be scaled independently

## Best Practices

1. **Idempotent Operations**: Ensure compensation operations are idempotent
2. **Event Ordering**: Handle event ordering properly
3. **Timeout Handling**: Implement appropriate timeouts
4. **Monitoring**: Monitor saga execution and compensation
5. **Testing**: Test both happy path and failure scenarios
