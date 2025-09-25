using Magnett.Automation.Core.IntegrationTest.StateMachines.PaymentMachine;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.PaymentMachine;

/// <summary>
/// Integration test for Payment Processing State Machine example.
/// This test demonstrates the complete workflow of payment processing
/// and validates that all state transitions work correctly.
/// </summary>
public class PaymentMachineTest
{
    [Fact]
    public void CreateDefinition_GetValidDefinition()
    {
        var definition = PaymentStateMachineDefinition.Definition;
            
        Assert.NotNull(definition);
        Assert.NotNull(definition.InitialState);
        Assert.Equal("Pending", definition.InitialState.Key.Name);
    }

    [Fact]
    public async Task Payment_Should_Transition_Through_Complete_Workflow()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create a payment
            var payment = new Payment("pay-1", 100.50m, "USD", "CreditCard", eventBus);
            Assert.Equal(PaymentState.Pending, payment.CurrentState);

            // Initialize the payment with its state machine
            await payment.InitializeAsync();
            Assert.Equal(PaymentState.Pending, payment.CurrentState);

            // Process payment
            var processed = await payment.ProcessAsync();
            Assert.True(processed);
            Assert.Equal(PaymentState.Processing, payment.CurrentState);

            // Complete payment
            var completed = await payment.CompleteAsync("txn-12345");
            Assert.True(completed);
            Assert.Equal(PaymentState.Completed, payment.CurrentState);
            Assert.Equal("txn-12345", payment.TransactionId);
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task Payment_Should_Handle_Failure_And_Retry_Workflow()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create and initialize payment
            var payment = new Payment("pay-2", 50.00m, "EUR", "DebitCard", eventBus);
            await payment.InitializeAsync();

            // Process payment
            await payment.ProcessAsync();
            Assert.Equal(PaymentState.Processing, payment.CurrentState);

            // Fail payment
            var failed = await payment.FailAsync("Insufficient funds");
            Assert.True(failed);
            Assert.Equal(PaymentState.Failed, payment.CurrentState);
            Assert.Equal("Insufficient funds", payment.ErrorMessage);

            // Retry payment
            var retried = await payment.RetryAsync();
            Assert.True(retried);
            Assert.Equal(PaymentState.Pending, payment.CurrentState);
            Assert.Equal(1, payment.RetryCount);
            Assert.Null(payment.ErrorMessage); // Error should be cleared
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task Payment_Should_Handle_Cancellation_Workflow()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create and initialize payment
            var payment = new Payment("pay-3", 75.25m, "GBP", "PayPal", eventBus);
            await payment.InitializeAsync();

            // Cancel payment from Pending state
            var cancelled = await payment.CancelAsync();
            Assert.True(cancelled);
            Assert.Equal(PaymentState.Cancelled, payment.CurrentState);
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task Payment_Should_Handle_Refund_Workflow()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create and initialize payment
            var payment = new Payment("pay-4", 200.00m, "USD", "BankTransfer", eventBus);
            await payment.InitializeAsync();

            // Process and complete payment
            await payment.ProcessAsync();
            await payment.CompleteAsync("txn-67890");
            Assert.Equal(PaymentState.Completed, payment.CurrentState);

            // Refund payment
            var refunded = await payment.RefundAsync();
            Assert.True(refunded);
            Assert.Equal(PaymentState.Refunded, payment.CurrentState);
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task Payment_Should_Handle_Cancellation_From_Failed_State()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create and initialize payment
            var payment = new Payment("pay-5", 150.75m, "CAD", "ApplePay", eventBus);
            await payment.InitializeAsync();

            // Process and fail payment
            await payment.ProcessAsync();
            await payment.FailAsync("Network timeout");
            Assert.Equal(PaymentState.Failed, payment.CurrentState);

            // Cancel payment from Failed state
            var cancelled = await payment.CancelAsync();
            Assert.True(cancelled);
            Assert.Equal(PaymentState.Cancelled, payment.CurrentState);
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task Payment_Should_Throw_Exception_When_Not_Initialized()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create payment but don't initialize
            var payment = new Payment("pay-6", 25.00m, "USD", "Cash", eventBus);

            // Should throw exception when trying to process without initialization
            await Assert.ThrowsAsync<InvalidOperationException>(() => payment.ProcessAsync());
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task Payment_Should_Handle_Invalid_Transitions_Gracefully()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create and initialize payment
            var payment = new Payment("pay-7", 300.00m, "JPY", "Crypto", eventBus);
            await payment.InitializeAsync();

            // Try to complete from Pending state (should fail)
            var completed = await payment.CompleteAsync("txn-invalid");
            Assert.False(completed); // Should return false, not throw exception
            Assert.Equal(PaymentState.Pending, payment.CurrentState); // Should remain in Pending
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }
}
