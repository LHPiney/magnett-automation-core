using System;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.UnitTest.Events.Fakes;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Events.Implementation;

public class EventContextTest
{
    [Fact]
    public void Constructor_WithValidEventAndProcessor_ShouldCreateInstance()
    {
        // Arrange
        var testEvent = new TestEvent("test_data");
        Func<EventContext, CancellationToken, Task> processor = (context, token) => Task.CompletedTask;

        // Act
        var context = new EventContext(testEvent, processor);

        // Assert
        Assert.NotNull(context);
        Assert.Equal(testEvent, context.Event);
        Assert.Equal(typeof(TestEvent), context.EventType);
        Assert.NotNull(context.Processor);
        Assert.True(context.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_WithNullEvent_ShouldThrowArgumentNullException()
    {
        // Arrange
        Func<EventContext, CancellationToken, Task> processor = (context, token) => Task.CompletedTask;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventContext(null!, processor));
    }

    [Fact]
    public void Constructor_WithNullProcessor_ShouldThrowArgumentNullException()
    {
        // Arrange
        var testEvent = new TestEvent("test_data");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventContext(testEvent, null!));
    }

    [Fact]
    public async Task ProcessAsync_ShouldCallProcessor()
    {
        // Arrange
        var testEvent = new TestEvent("test_data");
        var processorCalled = false;
        EventContext? receivedContext = null;
        Func<EventContext, CancellationToken, Task> processor = (context, token) =>
        {
            processorCalled = true;
            receivedContext = context;
            return Task.CompletedTask;
        };

        var context = new EventContext(testEvent, processor);

        // Act
        await context.ProcessAsync();

        // Assert
        Assert.True(processorCalled);
        Assert.Equal(context, receivedContext);
    }

    [Fact]
    public async Task ProcessAsync_WithCancellationToken_ShouldPassTokenToProcessor()
    {
        // Arrange
        var testEvent = new TestEvent("test_data");
        var cancellationTokenReceived = CancellationToken.None;
        Func<EventContext, CancellationToken, Task> processor = (context, token) =>
        {
            cancellationTokenReceived = token;
            return Task.CompletedTask;
        };

        var context = new EventContext(testEvent, processor);
        using var cts = new CancellationTokenSource();

        // Act
        await context.ProcessAsync(cts.Token);

        // Assert
        Assert.Equal(cts.Token, cancellationTokenReceived);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var testEvent = new TestEvent("test_data");
        Func<EventContext, CancellationToken, Task> processor = (context, token) => Task.CompletedTask;
        var context = new EventContext(testEvent, processor);

        // Act
        var result = context.ToString();

        // Assert
        Assert.Contains("EventContext", result);
        Assert.Contains("TestEvent", result);
        Assert.Contains("CreatedAt", result);
    }

    [Fact]
    public void EventType_ShouldReturnCorrectType()
    {
        // Arrange
        var testEvent = new TestEvent("test_data");
        Func<EventContext, CancellationToken, Task> processor = (context, token) => Task.CompletedTask;

        // Act
        var context = new EventContext(testEvent, processor);

        // Assert
        Assert.Equal(typeof(TestEvent), context.EventType);
    }

    [Fact]
    public void CreatedAt_ShouldBeSetToCurrentTime()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;
        var testEvent = new TestEvent("test_data");
        Func<EventContext, CancellationToken, Task> processor = (context, token) => Task.CompletedTask;

        // Act
        var context = new EventContext(testEvent, processor);
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(context.CreatedAt >= beforeCreation);
        Assert.True(context.CreatedAt <= afterCreation);
    }
}
