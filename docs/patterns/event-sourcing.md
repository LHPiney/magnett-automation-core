---
layout: default
title: Event Sourcing Pattern
nav_order: 1
parent: Patterns
permalink: /patterns/event-sourcing/
---

# Event Sourcing Pattern

Event Sourcing is a pattern where the state of an application is determined by a sequence of events rather than storing the current state directly. This approach provides a complete audit trail and enables powerful capabilities like time-travel debugging and event replay.

## Overview

In Event Sourcing, instead of updating a record in place, you append events to an event store. The current state is derived by replaying all events from the beginning or from a snapshot.

## Benefits

- **Complete Audit Trail**: Every change is recorded as an event
- **Time Travel**: Replay events to see state at any point in time
- **Debugging**: Understand exactly what happened and when
- **Scalability**: Events can be processed asynchronously
- **Integration**: Events naturally support system integration

## Implementation with Magnett Automation Core

### 1. Define Domain Events

```csharp
using Magnett.Automation.Core.Events;

// Domain events for an order
public record OrderCreatedEvent(string OrderId, decimal Amount, string CustomerId) 
    : Event("OrderCreated", "OrderService");

public record OrderItemAddedEvent(string OrderId, string ProductId, int Quantity, decimal Price) 
    : Event("OrderItemAdded", "OrderService");

public record OrderItemRemovedEvent(string OrderId, string ProductId) 
    : Event("OrderItemRemoved", "OrderService");

public record OrderAmountUpdatedEvent(string OrderId, decimal NewAmount) 
    : Event("OrderAmountUpdated", "OrderService");

public record OrderCancelledEvent(string OrderId, string Reason) 
    : Event("OrderCancelled", "OrderService");
```

### 2. Create Event Store

```csharp
using Magnett.Automation.Core.Events.Implementations;
using Microsoft.Extensions.Logging;

public class EventStore
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<EventStore> _logger;
    private readonly Dictionary<string, List<IEvent>> _events = new();

    public EventStore(IEventBus eventBus, ILogger<EventStore> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task AppendEventAsync(string aggregateId, IEvent @event)
    {
        if (!_events.ContainsKey(aggregateId))
        {
            _events[aggregateId] = new List<IEvent>();
        }

        _events[aggregateId].Add(@event);
        
        // Publish event for real-time processing
        await _eventBus.PublishAsync(@event);
        
        _logger.LogInformation("Event {EventType} appended for aggregate {AggregateId}", 
            @event.Name, aggregateId);
    }

    public IEnumerable<IEvent> GetEvents(string aggregateId)
    {
        return _events.TryGetValue(aggregateId, out var events) 
            ? events.AsReadOnly() 
            : Enumerable.Empty<IEvent>();
    }

    public IEnumerable<IEvent> GetEventsFromVersion(string aggregateId, int fromVersion)
    {
        var events = GetEvents(aggregateId);
        return events.Skip(fromVersion);
    }
}
```

### 3. Create Aggregate Root

```csharp
public class Order
{
    private readonly List<IEvent> _uncommittedEvents = new();
    
    public string Id { get; private set; }
    public decimal Amount { get; private set; }
    public string CustomerId { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();
    public OrderStatus Status { get; private set; }
    public int Version { get; private set; }

    private Order() { }

    public static Order Create(string orderId, string customerId, decimal initialAmount)
    {
        var order = new Order();
        order.Apply(new OrderCreatedEvent(orderId, initialAmount, customerId));
        return order;
    }

    public void AddItem(string productId, int quantity, decimal price)
    {
        if (Status != OrderStatus.Active)
            throw new InvalidOperationException("Cannot add items to inactive order");

        var item = new OrderItem(productId, quantity, price);
        Items.Add(item);
        
        var newAmount = Items.Sum(i => i.TotalPrice);
        Apply(new OrderItemAddedEvent(Id, productId, quantity, price));
        Apply(new OrderAmountUpdatedEvent(Id, newAmount));
    }

    public void RemoveItem(string productId)
    {
        if (Status != OrderStatus.Active)
            throw new InvalidOperationException("Cannot remove items from inactive order");

        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null) return;

        Items.Remove(item);
        
        var newAmount = Items.Sum(i => i.TotalPrice);
        Apply(new OrderItemRemovedEvent(Id, productId));
        Apply(new OrderAmountUpdatedEvent(Id, newAmount));
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Cancelled)
            return;

        Status = OrderStatus.Cancelled;
        Apply(new OrderCancelledEvent(Id, reason));
    }

    private void Apply(IEvent @event)
    {
        _uncommittedEvents.Add(@event);
        Handle(@event);
    }

    private void Handle(IEvent @event)
    {
        switch (@event)
        {
            case OrderCreatedEvent e:
                Id = e.OrderId;
                CustomerId = e.CustomerId;
                Amount = e.Amount;
                Status = OrderStatus.Active;
                Version = 0;
                break;
            case OrderAmountUpdatedEvent e:
                Amount = e.NewAmount;
                Version++;
                break;
            case OrderCancelledEvent _:
                Status = OrderStatus.Cancelled;
                Version++;
                break;
        }
    }

    public IEnumerable<IEvent> GetUncommittedEvents()
    {
        return _uncommittedEvents.AsReadOnly();
    }

    public void MarkEventsAsCommitted()
    {
        _uncommittedEvents.Clear();
    }

    // Replay events to rebuild state
    public static Order FromEvents(IEnumerable<IEvent> events)
    {
        var order = new Order();
        foreach (var @event in events)
        {
            order.Handle(@event);
        }
        return order;
    }
}

public class OrderItem
{
    public string ProductId { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }
    public decimal TotalPrice => Quantity * UnitPrice;

    public OrderItem(string productId, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}

public enum OrderStatus
{
    Active,
    Cancelled,
    Completed
}
```

### 4. Create Event Handlers

```csharp
using Magnett.Automation.Core.Events;
using Microsoft.Extensions.Logging;

public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderCreatedEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order {OrderId} created for customer {CustomerId} with amount {Amount}",
            @event.OrderId, @event.CustomerId, @event.Amount);
        
        // Here you could update read models, send notifications, etc.
        return Task.CompletedTask;
    }
}

public class OrderAmountUpdatedHandler : IEventHandler<OrderAmountUpdatedEvent>
{
    private readonly ILogger<OrderAmountUpdatedHandler> _logger;

    public OrderAmountUpdatedHandler(ILogger<OrderAmountUpdatedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderAmountUpdatedEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order {OrderId} amount updated to {Amount}",
            @event.OrderId, @event.NewAmount);
        
        // Update read models, trigger business rules, etc.
        return Task.CompletedTask;
    }
}
```

### 5. Create Repository

```csharp
public class OrderRepository
{
    private readonly EventStore _eventStore;
    private readonly IEventBus _eventBus;

    public OrderRepository(EventStore eventStore, IEventBus eventBus)
    {
        _eventStore = eventStore;
        _eventBus = eventBus;
    }

    public async Task<Order> GetByIdAsync(string orderId)
    {
        var events = _eventStore.GetEvents(orderId);
        return Order.FromEvents(events);
    }

    public async Task SaveAsync(Order order)
    {
        var uncommittedEvents = order.GetUncommittedEvents();
        
        foreach (var @event in uncommittedEvents)
        {
            await _eventStore.AppendEventAsync(order.Id, @event);
        }
        
        order.MarkEventsAsCommitted();
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string customerId)
    {
        // This would typically query a read model
        // For simplicity, we'll return empty for now
        return Enumerable.Empty<Order>();
    }
}
```

### 6. Usage Example

```csharp
public class OrderService
{
    private readonly OrderRepository _repository;
    private readonly IEventBus _eventBus;

    public OrderService(OrderRepository repository, IEventBus eventBus)
    {
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<string> CreateOrderAsync(string customerId, decimal initialAmount)
    {
        var orderId = Guid.NewGuid().ToString();
        var order = Order.Create(orderId, customerId, initialAmount);
        
        await _repository.SaveAsync(order);
        
        return orderId;
    }

    public async Task AddItemToOrderAsync(string orderId, string productId, int quantity, decimal price)
    {
        var order = await _repository.GetByIdAsync(orderId);
        order.AddItem(productId, quantity, price);
        
        await _repository.SaveAsync(order);
    }

    public async Task CancelOrderAsync(string orderId, string reason)
    {
        var order = await _repository.GetByIdAsync(orderId);
        order.Cancel(reason);
        
        await _repository.SaveAsync(order);
    }
}
```

## Advanced Features

### Snapshots

For aggregates with many events, you can create snapshots to improve performance:

```csharp
public class OrderSnapshot
{
    public string Id { get; set; }
    public decimal Amount { get; set; }
    public string CustomerId { get; set; }
    public OrderStatus Status { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SnapshotService
{
    public async Task<OrderSnapshot> CreateSnapshotAsync(Order order)
    {
        return new OrderSnapshot
        {
            Id = order.Id,
            Amount = order.Amount,
            CustomerId = order.CustomerId,
            Status = order.Status,
            Version = order.Version,
            CreatedAt = DateTime.UtcNow
        };
    }

    public Order RebuildFromSnapshot(OrderSnapshot snapshot, IEnumerable<IEvent> eventsAfterSnapshot)
    {
        var order = new Order();
        // Apply snapshot state
        order.Id = snapshot.Id;
        order.Amount = snapshot.Amount;
        order.CustomerId = snapshot.CustomerId;
        order.Status = snapshot.Status;
        order.Version = snapshot.Version;
        
        // Apply events after snapshot
        foreach (var @event in eventsAfterSnapshot)
        {
            order.Handle(@event);
        }
        
        return order;
    }
}
```

## Best Practices

1. **Immutable Events**: Events should be immutable once created
2. **Event Versioning**: Version your events for backward compatibility
3. **Idempotency**: Ensure event handlers are idempotent
4. **Snapshots**: Use snapshots for aggregates with many events
5. **Event Store**: Consider using a dedicated event store for production
6. **Projections**: Create read models optimized for queries

## View Complete Example

The fully functional example is available at:
- Source code: `test/Magnett.Automation.Core.IntegrationTest/Events/`
- Tests: `test/Magnett.Automation.Core.IntegrationTest/Events/EventTest.cs`

## Run Example

```bash
# Run all Event Sourcing tests
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

## When to Use Event Sourcing

- **Audit Requirements**: When you need complete audit trails
- **Complex Business Logic**: When business rules are complex and change frequently
- **Integration**: When multiple systems need to react to changes
- **Debugging**: When you need to understand what happened and when
- **Compliance**: When regulatory requirements demand event history
