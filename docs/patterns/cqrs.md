---
layout: default
title: CQRS Pattern
nav_order: 2
parent: Patterns
permalink: /patterns/cqrs/
---

# CQRS (Command Query Responsibility Segregation) Pattern

CQRS separates read and write operations by using different models for commands (writes) and queries (reads). This pattern allows for optimized data models and improved scalability.

## Overview

In CQRS, you separate the command model (used for writes) from the query model (used for reads). This allows each model to be optimized for its specific purpose.

## Benefits

- **Optimized Models**: Different models optimized for reads vs writes
- **Scalability**: Scale read and write operations independently
- **Performance**: Optimize queries without affecting write performance
- **Flexibility**: Use different storage technologies for reads and writes
- **Team Productivity**: Different teams can work on read and write models

## Implementation with Magnett Automation Core

### 1. Define Commands and Queries

```csharp
// Commands (Write operations)
public record CreateOrderCommand(string CustomerId, decimal Amount, string Description);
public record AddOrderItemCommand(string OrderId, string ProductId, int Quantity, decimal Price);
public record CancelOrderCommand(string OrderId, string Reason);
public record UpdateOrderStatusCommand(string OrderId, OrderStatus Status);

// Queries (Read operations)
public record GetOrderQuery(string OrderId);
public record GetOrdersByCustomerQuery(string CustomerId);
public record GetOrdersByStatusQuery(OrderStatus Status);
public record GetOrderSummaryQuery(string OrderId);

// Query Results
public record OrderReadModel
{
    public string Id { get; init; }
    public string CustomerId { get; init; }
    public decimal Amount { get; init; }
    public string Description { get; init; }
    public OrderStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<OrderItemReadModel> Items { get; init; } = new();
}

public record OrderItemReadModel
{
    public string ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice { get; init; }
}

public record OrderSummaryReadModel
{
    public string Id { get; init; }
    public string CustomerId { get; init; }
    public decimal Amount { get; init; }
    public OrderStatus Status { get; init; }
    public int ItemCount { get; init; }
}
```

### 2. Create Command Handlers

```csharp
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;

public class CreateOrderCommandHandler
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(IEventBus eventBus, ILogger<CreateOrderCommandHandler> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<string> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        var orderId = Guid.NewGuid().ToString();
        
        var orderCreatedEvent = new OrderCreatedEvent(orderId, command.CustomerId, command.Amount, command.Description);
        
        await _eventBus.PublishAsync(orderCreatedEvent);
        
        _logger.LogInformation("Order {OrderId} created for customer {CustomerId}", 
            orderId, command.CustomerId);
        
        return orderId;
    }
}

public class AddOrderItemCommandHandler
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<AddOrderItemCommandHandler> _logger;

    public AddOrderItemCommandHandler(IEventBus eventBus, ILogger<AddOrderItemCommandHandler> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task HandleAsync(AddOrderItemCommand command, CancellationToken cancellationToken = default)
    {
        var orderItemAddedEvent = new OrderItemAddedEvent(
            command.OrderId, 
            command.ProductId, 
            command.Quantity, 
            command.Price);
        
        await _eventBus.PublishAsync(orderItemAddedEvent);
        
        _logger.LogInformation("Item {ProductId} added to order {OrderId}", 
            command.ProductId, command.OrderId);
    }
}

public class CancelOrderCommandHandler
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<CancelOrderCommandHandler> _logger;

    public CancelOrderCommandHandler(IEventBus eventBus, ILogger<CancelOrderCommandHandler> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task HandleAsync(CancelOrderCommand command, CancellationToken cancellationToken = default)
    {
        var orderCancelledEvent = new OrderCancelledEvent(command.OrderId, command.Reason);
        
        await _eventBus.PublishAsync(orderCancelledEvent);
        
        _logger.LogInformation("Order {OrderId} cancelled: {Reason}", 
            command.OrderId, command.Reason);
    }
}
```

### 3. Create Query Handlers

```csharp
public class GetOrderQueryHandler
{
    private readonly IOrderReadRepository _readRepository;
    private readonly ILogger<GetOrderQueryHandler> _logger;

    public GetOrderQueryHandler(IOrderReadRepository readRepository, ILogger<GetOrderQueryHandler> logger)
    {
        _readRepository = readRepository;
        _logger = logger;
    }

    public async Task<OrderReadModel?> HandleAsync(GetOrderQuery query, CancellationToken cancellationToken = default)
    {
        var order = await _readRepository.GetByIdAsync(query.OrderId);
        
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", query.OrderId);
            return null;
        }
        
        return order;
    }
}

public class GetOrdersByCustomerQueryHandler
{
    private readonly IOrderReadRepository _readRepository;
    private readonly ILogger<GetOrdersByCustomerQueryHandler> _logger;

    public GetOrdersByCustomerQueryHandler(IOrderReadRepository readRepository, ILogger<GetOrdersByCustomerQueryHandler> logger)
    {
        _readRepository = readRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderReadModel>> HandleAsync(GetOrdersByCustomerQuery query, CancellationToken cancellationToken = default)
    {
        var orders = await _readRepository.GetByCustomerIdAsync(query.CustomerId);
        
        _logger.LogInformation("Found {Count} orders for customer {CustomerId}", 
            orders.Count(), query.CustomerId);
        
        return orders;
    }
}

public class GetOrderSummaryQueryHandler
{
    private readonly IOrderReadRepository _readRepository;
    private readonly ILogger<GetOrderSummaryQueryHandler> _logger;

    public GetOrderSummaryQueryHandler(IOrderReadRepository readRepository, ILogger<GetOrderSummaryQueryHandler> logger)
    {
        _readRepository = readRepository;
        _logger = logger;
    }

    public async Task<OrderSummaryReadModel?> HandleAsync(GetOrderSummaryQuery query, CancellationToken cancellationToken = default)
    {
        var order = await _readRepository.GetByIdAsync(query.OrderId);
        
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found for summary", query.OrderId);
            return null;
        }
        
        return new OrderSummaryReadModel
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Amount = order.Amount,
            Status = order.Status,
            ItemCount = order.Items.Count
        };
    }
}
```

### 4. Create Read Repository

```csharp
public interface IOrderReadRepository
{
    Task<OrderReadModel?> GetByIdAsync(string orderId);
    Task<IEnumerable<OrderReadModel>> GetByCustomerIdAsync(string customerId);
    Task<IEnumerable<OrderReadModel>> GetByStatusAsync(OrderStatus status);
    Task<IEnumerable<OrderReadModel>> GetAllAsync();
}

public class OrderReadRepository : IOrderReadRepository
{
    private readonly Dictionary<string, OrderReadModel> _orders = new();
    private readonly ILogger<OrderReadRepository> _logger;

    public OrderReadRepository(ILogger<OrderReadRepository> logger)
    {
        _logger = logger;
    }

    public Task<OrderReadModel?> GetByIdAsync(string orderId)
    {
        _orders.TryGetValue(orderId, out var order);
        return Task.FromResult(order);
    }

    public Task<IEnumerable<OrderReadModel>> GetByCustomerIdAsync(string customerId)
    {
        var orders = _orders.Values.Where(o => o.CustomerId == customerId);
        return Task.FromResult(orders);
    }

    public Task<IEnumerable<OrderReadModel>> GetByStatusAsync(OrderStatus status)
    {
        var orders = _orders.Values.Where(o => o.Status == status);
        return Task.FromResult(orders);
    }

    public Task<IEnumerable<OrderReadModel>> GetAllAsync()
    {
        return Task.FromResult(_orders.Values.AsEnumerable());
    }

    public void UpdateOrder(OrderReadModel order)
    {
        _orders[order.Id] = order;
        _logger.LogInformation("Order {OrderId} updated in read model", order.Id);
    }

    public void AddOrder(OrderReadModel order)
    {
        _orders[order.Id] = order;
        _logger.LogInformation("Order {OrderId} added to read model", order.Id);
    }
}
```

### 5. Create Event Handlers for Read Model Updates

```csharp
public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly OrderReadRepository _readRepository;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(OrderReadRepository readRepository, ILogger<OrderCreatedEventHandler> logger)
    {
        _readRepository = readRepository;
        _logger = logger;
    }

    public Task Handle(OrderCreatedEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        var orderReadModel = new OrderReadModel
        {
            Id = @event.OrderId,
            CustomerId = @event.CustomerId,
            Amount = @event.Amount,
            Description = @event.Description,
            Status = OrderStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Items = new List<OrderItemReadModel>()
        };

        _readRepository.AddOrder(orderReadModel);
        
        _logger.LogInformation("Order {OrderId} added to read model", @event.OrderId);
        
        return Task.CompletedTask;
    }
}

public class OrderItemAddedEventHandler : IEventHandler<OrderItemAddedEvent>
{
    private readonly OrderReadRepository _readRepository;
    private readonly ILogger<OrderItemAddedEventHandler> _logger;

    public OrderItemAddedEventHandler(OrderReadRepository readRepository, ILogger<OrderItemAddedEventHandler> logger)
    {
        _readRepository = readRepository;
        _logger = logger;
    }

    public Task Handle(OrderItemAddedEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        var order = _readRepository.GetByIdAsync(@event.OrderId).Result;
        if (order == null) return Task.CompletedTask;

        var updatedOrder = order with
        {
            Items = order.Items.Concat(new[]
            {
                new OrderItemReadModel
                {
                    ProductId = @event.ProductId,
                    Quantity = @event.Quantity,
                    UnitPrice = @event.Price,
                    TotalPrice = @event.Quantity * @event.Price
                }
            }).ToList(),
            Amount = order.Amount + (@event.Quantity * @event.Price),
            UpdatedAt = DateTime.UtcNow
        };

        _readRepository.UpdateOrder(updatedOrder);
        
        _logger.LogInformation("Order {OrderId} updated in read model", @event.OrderId);
        
        return Task.CompletedTask;
    }
}

public class OrderCancelledEventHandler : IEventHandler<OrderCancelledEvent>
{
    private readonly OrderReadRepository _readRepository;
    private readonly ILogger<OrderCancelledEventHandler> _logger;

    public OrderCancelledEventHandler(OrderReadRepository readRepository, ILogger<OrderCancelledEventHandler> logger)
    {
        _readRepository = readRepository;
        _logger = logger;
    }

    public Task Handle(OrderCancelledEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        var order = _readRepository.GetByIdAsync(@event.OrderId).Result;
        if (order == null) return Task.CompletedTask;

        var updatedOrder = order with
        {
            Status = OrderStatus.Cancelled,
            UpdatedAt = DateTime.UtcNow
        };

        _readRepository.UpdateOrder(updatedOrder);
        
        _logger.LogInformation("Order {OrderId} cancelled in read model", @event.OrderId);
        
        return Task.CompletedTask;
    }
}
```

### 6. Create Application Service

```csharp
public class OrderApplicationService
{
    private readonly CreateOrderCommandHandler _createOrderHandler;
    private readonly AddOrderItemCommandHandler _addItemHandler;
    private readonly CancelOrderCommandHandler _cancelOrderHandler;
    private readonly GetOrderQueryHandler _getOrderHandler;
    private readonly GetOrdersByCustomerQueryHandler _getOrdersByCustomerHandler;
    private readonly GetOrderSummaryQueryHandler _getOrderSummaryHandler;

    public OrderApplicationService(
        CreateOrderCommandHandler createOrderHandler,
        AddOrderItemCommandHandler addItemHandler,
        CancelOrderCommandHandler cancelOrderHandler,
        GetOrderQueryHandler getOrderHandler,
        GetOrdersByCustomerQueryHandler getOrdersByCustomerHandler,
        GetOrderSummaryQueryHandler getOrderSummaryHandler)
    {
        _createOrderHandler = createOrderHandler;
        _addItemHandler = addItemHandler;
        _cancelOrderHandler = cancelOrderHandler;
        _getOrderHandler = getOrderHandler;
        _getOrdersByCustomerHandler = getOrdersByCustomerHandler;
        _getOrderSummaryHandler = getOrderSummaryHandler;
    }

    // Command operations
    public async Task<string> CreateOrderAsync(string customerId, decimal amount, string description)
    {
        var command = new CreateOrderCommand(customerId, amount, description);
        return await _createOrderHandler.HandleAsync(command);
    }

    public async Task AddItemToOrderAsync(string orderId, string productId, int quantity, decimal price)
    {
        var command = new AddOrderItemCommand(orderId, productId, quantity, price);
        await _addItemHandler.HandleAsync(command);
    }

    public async Task CancelOrderAsync(string orderId, string reason)
    {
        var command = new CancelOrderCommand(orderId, reason);
        await _cancelOrderHandler.HandleAsync(command);
    }

    // Query operations
    public async Task<OrderReadModel?> GetOrderAsync(string orderId)
    {
        var query = new GetOrderQuery(orderId);
        return await _getOrderHandler.HandleAsync(query);
    }

    public async Task<IEnumerable<OrderReadModel>> GetOrdersByCustomerAsync(string customerId)
    {
        var query = new GetOrdersByCustomerQuery(customerId);
        return await _getOrdersByCustomerHandler.HandleAsync(query);
    }

    public async Task<OrderSummaryReadModel?> GetOrderSummaryAsync(string orderId)
    {
        var query = new GetOrderSummaryQuery(orderId);
        return await _getOrderSummaryHandler.HandleAsync(query);
    }
}
```

### 7. Usage Example

```csharp
public class OrderController
{
    private readonly OrderApplicationService _orderService;

    public OrderController(OrderApplicationService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var orderId = await _orderService.CreateOrderAsync(
            request.CustomerId, 
            request.Amount, 
            request.Description);
        
        return Ok(new { OrderId = orderId });
    }

    [HttpGet("orders/{orderId}")]
    public async Task<IActionResult> GetOrder(string orderId)
    {
        var order = await _orderService.GetOrderAsync(orderId);
        
        if (order == null)
            return NotFound();
        
        return Ok(order);
    }

    [HttpGet("customers/{customerId}/orders")]
    public async Task<IActionResult> GetOrdersByCustomer(string customerId)
    {
        var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
        return Ok(orders);
    }

    [HttpPost("orders/{orderId}/items")]
    public async Task<IActionResult> AddItem(string orderId, [FromBody] AddItemRequest request)
    {
        await _orderService.AddItemToOrderAsync(
            orderId, 
            request.ProductId, 
            request.Quantity, 
            request.Price);
        
        return Ok();
    }

    [HttpPost("orders/{orderId}/cancel")]
    public async Task<IActionResult> CancelOrder(string orderId, [FromBody] CancelOrderRequest request)
    {
        await _orderService.CancelOrderAsync(orderId, request.Reason);
        return Ok();
    }
}
```

## Advanced CQRS Patterns

### Eventual Consistency

```csharp
public class ConsistencyChecker
{
    private readonly IOrderReadRepository _readRepository;
    private readonly IEventStore _eventStore;
    private readonly ILogger<ConsistencyChecker> _logger;

    public ConsistencyChecker(
        IOrderReadRepository readRepository, 
        IEventStore eventStore, 
        ILogger<ConsistencyChecker> logger)
    {
        _readRepository = readRepository;
        _eventStore = eventStore;
        _logger = logger;
    }

    public async Task CheckAndRepairConsistencyAsync(string orderId)
    {
        var readModel = await _readRepository.GetByIdAsync(orderId);
        var events = _eventStore.GetEvents(orderId);
        
        // Rebuild read model from events
        var expectedReadModel = RebuildReadModelFromEvents(events);
        
        if (!AreEqual(readModel, expectedReadModel))
        {
            _logger.LogWarning("Inconsistency detected for order {OrderId}, repairing...", orderId);
            _readRepository.UpdateOrder(expectedReadModel);
        }
    }

    private OrderReadModel RebuildReadModelFromEvents(IEnumerable<IEvent> events)
    {
        // Implementation to rebuild read model from events
        // This would be similar to the aggregate's FromEvents method
        throw new NotImplementedException();
    }

    private bool AreEqual(OrderReadModel? readModel, OrderReadModel expectedReadModel)
    {
        if (readModel == null) return false;
        
        return readModel.Id == expectedReadModel.Id &&
               readModel.CustomerId == expectedReadModel.CustomerId &&
               readModel.Amount == expectedReadModel.Amount &&
               readModel.Status == expectedReadModel.Status;
    }
}
```

## Best Practices

1. **Separate Models**: Keep command and query models completely separate
2. **Eventual Consistency**: Accept that read models may be eventually consistent
3. **Optimize for Use Cases**: Optimize read models for specific query patterns
4. **Handle Failures**: Implement proper error handling and retry mechanisms
5. **Monitor Consistency**: Implement consistency checking and repair mechanisms
6. **Version Events**: Version your events for backward compatibility

## View Complete Example

The fully functional example is available at:
- Source code: `test/Magnett.Automation.Core.IntegrationTest/StateMachines/OrderMachine/`
- Tests: `test/Magnett.Automation.Core.IntegrationTest/StateMachines/OrderMachine/OrderMachineTest.cs`

## Run Example

```bash
# Run all Order Machine tests (CQRS flavor)
dotnet test test/Magnett.Automation.Core.IntegrationTest/Magnett.Automation.Core.IntegrationTest.csproj --filter "OrderMachineTest" --verbosity minimal

# Run specific test
dotnet test test/Magnett.Automation.Core.IntegrationTest/Magnett.Automation.Core.IntegrationTest.csproj --filter "OrderMachineTest_WhenCreateOrder_ShouldTransitionToPending" --verbosity minimal
```

## Example Structure

```
StateMachines/OrderMachine/
├── Order.cs
├── OrderState.cs
├── OrderAction.cs
├── OrderStateMachine.cs
├── OrderStateMachineDefinition.cs
└── OrderMachineTest.cs
```

## When to Use CQRS

- **Different Read/Write Patterns**: When read and write patterns are very different
- **Performance Requirements**: When you need to optimize reads independently
- **Team Structure**: When different teams work on read and write sides
- **Scalability**: When you need to scale reads and writes independently
- **Complex Queries**: When you need complex query capabilities without affecting writes
