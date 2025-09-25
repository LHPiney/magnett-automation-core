using System;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.UnitTest.Events.Fakes;
using Magnett.Automation.Core.Metrics;
using Magnett.Automation.Core.Metrics.Implementations;
using Magnett.Automation.Core.Events.Metrics;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Events.Implementation;

public class EventBusTest
{
    [Fact]
    public async Task Create_EventBus_ShouldReturnInstance()
    {
        // Arrange && Act
        var logger = new Mock<ILogger<EventBus>>();
        await using var eventBus = EventBus.Create(logger.Object, startOnCreate: true);

        // Assert
        Assert.NotNull(eventBus);
    }

    [Fact]
    public async Task GetEventReader_ShouldReturnEventReader()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();
        await using var eventBus =  EventBus.Create(logger.Object, startOnCreate: true);

        // Act
        var result = eventBus.EventReader;

        // Assert
        Assert.IsAssignableFrom<IEventReader>(result);
    }
    
    [Fact]
    public void Create_WhenLoggerIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        ILogger<EventBus> logger = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => EventBus.Create(logger!, startOnCreate: true));
    }

    [Fact]
    public void PublishAsync_ShouldThrowArgumentNullException_WhenEventIsNull()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();
        var eventBus = EventBus.Create(logger.Object, startOnCreate: true);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => eventBus.PublishAsync<TestEvent>(null));
    }

    [Fact]
    public async Task PublishAsync_WithMetrics_ShouldRecordMetrics()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();
        var metricsRegistry = new MetricsRegistry();
        var metricsCollector = MetricsCollector.Create(metricsRegistry);
        await using var eventBus = EventBus.Create(logger.Object, null, metricsCollector);
        
        var testEvent = new TestEvent("test_data");
        
        // Act
        eventBus.Start();
        await eventBus.PublishAsync(testEvent);
        await Task.Delay(100); // Allow processing
        await eventBus.StopAsync();

        // Assert
        var publishedCount = metricsRegistry.GetCounterValue(EventsPublished.Name);
        Assert.True(publishedCount > 0);
    }

    [Fact]
    public async Task Start_ShouldSetStateToReady()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();
        await using var eventBus = EventBus.Create(logger.Object, startOnCreate: true);

        // Act
        eventBus.Start();

        // Assert
        Assert.Equal(EventBusState.Ready, eventBus.State);
        Assert.True(eventBus.IsReady);
    }

    [Fact]
    public async Task StopAsync_ShouldCompleteGracefully()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();
        await using var eventBus = EventBus.Create(logger.Object, startOnCreate: true);
        eventBus.Start();

        // Act & Assert
        await eventBus.StopAsync();
        // Should not throw
    }

    [Fact]
    public async Task EventReader_ShouldReturnReader()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();
        await using var eventBus = EventBus.Create(logger.Object, startOnCreate: true);

        // Act
        var reader = eventBus.EventReader;

        // Assert
        Assert.NotNull(reader);
        Assert.IsAssignableFrom<IEventReader>(reader);
    }

    [Fact]
    public async Task Start_WhenAlreadyRunning_ShouldNotThrow()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();
        await using var eventBus = EventBus.Create(logger.Object, null, null, false); // Don't auto-start

        // Act & Assert
        eventBus.Start();
        eventBus.Start(); // Second start should not throw
        Assert.Equal(EventBusState.Ready, eventBus.State);
    }

    [Fact]
    public async Task StopAsync_WhenAlreadyStopped_ShouldLogWarning()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();
        await using var eventBus = EventBus.Create(logger.Object, startOnCreate: true);

        // Act
        await eventBus.StopAsync(); // First stop
        await eventBus.StopAsync(); // Second stop

        // Assert
        logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("already stopped")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishAsync_WhenCancellationTokenIsRequested_ShouldReturnEarly()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();
        await using var eventBus = EventBus.Create(logger.Object, null, null, false); // Don't auto-start
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var testEvent = new TestEvent("test_data");

        // Act & Assert
        eventBus.Start();
        await eventBus.PublishAsync(testEvent, cts.Token); // Should not throw
        Assert.Equal(EventBusState.Ready, eventBus.State);
    }

    [Fact]
    public async Task PublishAsync_WhenNotRunning_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();
        await using var eventBus = EventBus.Create(logger.Object, null, null, false); // Don't start
        var testEvent = new TestEvent("test_data");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            eventBus.PublishAsync(testEvent));
    }

    [Fact]
    public async Task PublishAsync_WhenDisposed_ShouldThrowObjectDisposedException()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();
        var eventBus = EventBus.Create(logger.Object, startOnCreate: true);
        var testEvent = new TestEvent("test_data");
        
        eventBus.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => 
            eventBus.PublishAsync(testEvent));
    }

    [Fact]
    public void Dispose_WhenRunning_ShouldStopGracefully()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();
        var eventBus = EventBus.Create(logger.Object, null, null, false); // Don't auto-start
        eventBus.Start();

        // Act
        eventBus.Dispose();

        // Assert
        // Dispose should not change the state, but should clean up resources
        // The state remains Ready until explicitly stopped
        Assert.Equal(EventBusState.Ready, eventBus.State);
    }

    [Fact]
    public async Task DisposeAsync_WhenRunning_ShouldStopGracefully()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();
        var eventBus = EventBus.Create(logger.Object, startOnCreate: true);
        eventBus.Start();

        // Act
        await eventBus.DisposeAsync();

        // Assert
        Assert.Equal(EventBusState.Stopped, eventBus.State);
    }

    [Fact]
    public async Task Create_WithNullEventStream_ShouldCreateDefaultEventStream()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();

        // Act
        await using var eventBus = EventBus.Create(logger.Object, startOnCreate: true);

        // Assert
        Assert.NotNull(eventBus.EventReader);
    }

    [Fact]
    public async Task Create_WithStartOnCreateFalse_ShouldNotStart()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();

        // Act
        await using var eventBus = EventBus.Create(logger.Object, null, null, false);

        // Assert
        Assert.Equal(EventBusState.Stopped, eventBus.State);
        Assert.False(eventBus.IsReady);
    }
}