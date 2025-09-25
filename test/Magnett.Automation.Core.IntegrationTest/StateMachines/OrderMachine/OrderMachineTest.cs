using Magnett.Automation.Core.IntegrationTest.StateMachines.OrderMachine;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.OrderMachine;

/// <summary>
/// Integration test for Order Management State Machine example.
/// This test demonstrates the complete workflow of order management
/// and validates that all state transitions work correctly.
/// </summary>
public class OrderMachineTest
{
    [Fact]
    public void CreateDefinition_GetValidDefinition()
    {
        var definition = OrderStateMachineDefinition.Definition;
            
        Assert.NotNull(definition);
        Assert.NotNull(definition.InitialState);
        Assert.Equal("Pending", definition.InitialState.Key.Name);
    }

    [Fact]
    public async Task Order_Should_Transition_Through_Complete_Workflow()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create an order
            var items = new List<string> { "Laptop", "Mouse", "Keyboard" };
            var order = new Order("ord-1", "customer-1", items, 1299.99m, eventBus);
            Assert.Equal(OrderState.Pending, order.CurrentState);

            // Initialize the order with its state machine
            await order.InitializeAsync();
            Assert.Equal(OrderState.Pending, order.CurrentState);

            // Confirm order
            var confirmed = await order.ConfirmAsync();
            Assert.True(confirmed);
            Assert.Equal(OrderState.Confirmed, order.CurrentState);

            // Start processing
            var processing = await order.StartProcessingAsync();
            Assert.True(processing);
            Assert.Equal(OrderState.Processing, order.CurrentState);

            // Ship order
            var shipped = await order.ShipAsync("TRK123456789", "123 Main St, City, State");
            Assert.True(shipped);
            Assert.Equal(OrderState.Shipped, order.CurrentState);
            Assert.Equal("TRK123456789", order.TrackingNumber);
            Assert.Equal("123 Main St, City, State", order.ShippingAddress);

            // Deliver order
            var delivered = await order.DeliverAsync();
            Assert.True(delivered);
            Assert.Equal(OrderState.Delivered, order.CurrentState);
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task Order_Should_Handle_Cancellation_From_Multiple_States()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Test cancellation from Pending state
            var order1 = new Order("ord-2", "customer-2", new List<string> { "Book" }, 29.99m, eventBus);
            await order1.InitializeAsync();
            
            var cancelled1 = await order1.CancelAsync("Customer requested cancellation");
            Assert.True(cancelled1);
            Assert.Equal(OrderState.Cancelled, order1.CurrentState);
            Assert.Equal("Customer requested cancellation", order1.CancellationReason);

            // Test cancellation from Confirmed state
            var order2 = new Order("ord-3", "customer-3", new List<string> { "Phone" }, 599.99m, eventBus);
            await order2.InitializeAsync();
            await order2.ConfirmAsync();
            
            var cancelled2 = await order2.CancelAsync("Out of stock");
            Assert.True(cancelled2);
            Assert.Equal(OrderState.Cancelled, order2.CurrentState);

            // Test cancellation from Processing state
            var order3 = new Order("ord-4", "customer-4", new List<string> { "Tablet" }, 399.99m, eventBus);
            await order3.InitializeAsync();
            await order3.ConfirmAsync();
            await order3.StartProcessingAsync();
            
            var cancelled3 = await order3.CancelAsync("Quality issue");
            Assert.True(cancelled3);
            Assert.Equal(OrderState.Cancelled, order3.CurrentState);
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task Order_Should_Handle_Return_Workflow()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create and process order to delivered state
            var items = new List<string> { "Headphones", "Charger" };
            var order = new Order("ord-5", "customer-5", items, 199.99m, eventBus);
            await order.InitializeAsync();

            // Complete the order workflow
            await order.ConfirmAsync();
            await order.StartProcessingAsync();
            await order.ShipAsync("TRK987654321", "456 Oak Ave, Town, State");
            await order.DeliverAsync();
            Assert.Equal(OrderState.Delivered, order.CurrentState);

            // Return order
            var returned = await order.ReturnAsync("Defective product");
            Assert.True(returned);
            Assert.Equal(OrderState.Returned, order.CurrentState);
            Assert.Equal("Defective product", order.ReturnReason);
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task Order_Should_Throw_Exception_When_Not_Initialized()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create order but don't initialize
            var items = new List<string> { "Camera" };
            var order = new Order("ord-6", "customer-6", items, 799.99m, eventBus);

            // Should throw exception when trying to confirm without initialization
            await Assert.ThrowsAsync<InvalidOperationException>(() => order.ConfirmAsync());
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task Order_Should_Handle_Invalid_Transitions_Gracefully()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create and initialize order
            var items = new List<string> { "Watch" };
            var order = new Order("ord-7", "customer-7", items, 299.99m, eventBus);
            await order.InitializeAsync();

            // Try to ship from Pending state (should fail)
            var shipped = await order.ShipAsync("TRK-INVALID", "Invalid Address");
            Assert.False(shipped); // Should return false, not throw exception
            Assert.Equal(OrderState.Pending, order.CurrentState); // Should remain in Pending
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }
}
