using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Channels;
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

public class EventSystemIntegrationTest
{
    [Fact]
    public async Task CompleteEventFlow_WithMetrics_ShouldWorkCorrectly()
    {
        // Arrange
        var busLogger = new Mock<ILogger<EventBus>>();
        var streamLogger = new Mock<ILogger<EventStream>>();
        var metricsRegistry = new MetricsRegistry();
        var metricsCollector = MetricsCollector.Create(metricsRegistry);

        await using var eventBus = EventBus.Create(busLogger.Object, null, metricsCollector);
        await using var eventStream = EventStream.Create(streamLogger.Object);

        var testEvent = new TestEvent("integration_test_data");

        // Register handlers
        eventBus.EventHandlerRegistry.Register<TestEventHandler>();
        eventBus.EventHandlerRegistry.Register<SecondTestEventHandler>();

        // Start event bus
        eventBus.Start();

        // Act
        await eventBus.PublishAsync(testEvent);
        await Task.Delay(200); // Allow processing
        await eventBus.StopAsync();

        // Assert
        var publishedCount = metricsRegistry.GetCounterValue(EventsPublished.Name);
        var processedCount = metricsRegistry.GetCounterValue(EventsProcessed.Name);
        var failedCount = metricsRegistry.GetCounterValue(EventsFailed.Name);

        Assert.True(publishedCount > 0);
        Assert.True(processedCount > 0);
        Assert.Equal(0, failedCount); // Should not have failures
    }

    [Fact]
    public async Task MultipleEvents_WithMultipleHandlers_ShouldProcessAll()
    {
        // Arrange
        var busLogger = new Mock<ILogger<EventBus>>();
        var metricsRegistry = new MetricsRegistry();
        var metricsCollector = MetricsCollector.Create(metricsRegistry);

        await using var eventBus = EventBus.Create(busLogger.Object, null, metricsCollector);

        // Register multiple handlers
        eventBus.EventHandlerRegistry.Register<TestEventHandler>();
        eventBus.EventHandlerRegistry.Register<SecondTestEventHandler>();

        var events = new[]
        {
            new TestEvent("event1"),
            new TestEvent("event2"),
            new TestEvent("event3")
        };

        // Start event bus
        eventBus.Start();

        // Act
        foreach (var evt in events)
        {
            await eventBus.PublishAsync(evt);
        }

        await Task.Delay(300); // Allow processing
        await eventBus.StopAsync();

        // Assert
        var publishedCount = metricsRegistry.GetCounterValue(EventsPublished.Name);
        var processedCount = metricsRegistry.GetCounterValue(EventsProcessed.Name);

        Assert.Equal(events.Length, publishedCount);
        Assert.Equal(events.Length, processedCount);
    }

    [Fact]
    public async Task EventStream_ShouldPersistEvents()
    {
        // Arrange
        var streamLogger = new Mock<ILogger<EventStream>>();
        await using var eventStream = EventStream.Create(streamLogger.Object);

        var testEvent = new TestEvent("stream_test_data");
        var persistedEvents = new List<IEvent>();

        // Start reading from stream
        var readerTask = Task.Run(async () =>
        {
            await foreach (var evt in eventStream.Reader.ReadAllAsync())
            {
                persistedEvents.Add(evt);
            }
        });

        // Act
        await eventStream.Writer.WriteAsync(testEvent);
        await eventStream.Writer.WriteAsync(testEvent);
        eventStream.Writer.Complete();

        await readerTask;

        // Assert
        Assert.Equal(2, persistedEvents.Count);
        Assert.All(persistedEvents, evt => Assert.Equal(testEvent, evt));
    }

    [Fact]
    public async Task EventBus_WithEventStream_ShouldPersistAndProcess()
    {
        // Arrange
        var busLogger = new Mock<ILogger<EventBus>>();
        var streamLogger = new Mock<ILogger<EventStream>>();
        var metricsRegistry = new MetricsRegistry();
        var metricsCollector = MetricsCollector.Create(metricsRegistry);

        await using var eventStream = EventStream.Create(streamLogger.Object);
        await using var eventBus = EventBus.Create(busLogger.Object, eventStream, metricsCollector);

        var testEvent = new TestEvent("bus_stream_test");
        var persistedEvents = new List<IEvent>();

        // Register handler
        eventBus.EventHandlerRegistry.Register<TestEventHandler>();

        // Start reading from stream
        var readerTask = Task.Run(async () =>
        {
            await foreach (var evt in eventStream.Reader.ReadAllAsync())
            {
                persistedEvents.Add(evt);
            }
        });

        // Start event bus
        eventBus.Start();

        // Act
        await eventBus.PublishAsync(testEvent);
        await Task.Delay(100); // Allow processing
        await eventBus.StopAsync();

        // Complete the stream writer gracefully
        try
        {
            eventStream.Writer.Complete();
        }
        catch (ChannelClosedException)
        {
            // Channel already closed, which is fine
        }
        
        await readerTask;

        // Assert
        var publishedCount = metricsRegistry.GetCounterValue(EventsPublished.Name);
        var processedCount = metricsRegistry.GetCounterValue(EventsProcessed.Name);

        Assert.True(publishedCount > 0);
        Assert.True(processedCount > 0);
        Assert.Single(persistedEvents);
        Assert.Equal(testEvent, persistedEvents[0]);
    }

    [Fact]
    public async Task ConcurrentEventPublishing_ShouldHandleCorrectly()
    {
        // Arrange
        var busLogger = new Mock<ILogger<EventBus>>();
        var metricsRegistry = new MetricsRegistry();
        var metricsCollector = MetricsCollector.Create(metricsRegistry);

        await using var eventBus = EventBus.Create(busLogger.Object, null, metricsCollector);

        // Register handler
        eventBus.EventHandlerRegistry.Register<TestEventHandler>();

        const int eventCount = 10;
        var events = Enumerable.Range(0, eventCount)
            .Select(i => new TestEvent($"concurrent_event_{i}"))
            .ToArray();

        // Start event bus
        eventBus.Start();

        // Act
        var publishTasks = events.Select(evt => eventBus.PublishAsync(evt));
        await Task.WhenAll(publishTasks);

        await Task.Delay(500); // Allow processing
        await eventBus.StopAsync();

        // Assert
        var publishedCount = metricsRegistry.GetCounterValue(EventsPublished.Name);
        var processedCount = metricsRegistry.GetCounterValue(EventsProcessed.Name);

        Assert.Equal(eventCount, publishedCount);
        Assert.Equal(eventCount, processedCount);
    }

    [Fact]
    public async Task EventBus_StateTransitions_ShouldWorkCorrectly()
    {
        // Arrange
        var busLogger = new Mock<ILogger<EventBus>>();
        await using var eventBus = EventBus.Create(busLogger.Object);

        // Act & Assert
        Assert.Equal(EventBusState.Ready, eventBus.State);
        Assert.True(eventBus.IsReady);

        await eventBus.StopAsync();
        // State should be Stopped after stopping
        Assert.Equal(EventBusState.Stopped, eventBus.State);
    }

    [Fact]
    public async Task Metrics_ShouldTrackProcessingTime()
    {
        // Arrange
        var busLogger = new Mock<ILogger<EventBus>>();
        var metricsRegistry = new MetricsRegistry();
        var metricsCollector = MetricsCollector.Create(metricsRegistry);

        await using var eventBus = EventBus.Create(busLogger.Object, null, metricsCollector);

        // Register handler with delay
        eventBus.EventHandlerRegistry.Register<TestEventHandler>();

        var testEvent = new TestEvent("timing_test");

        // Start event bus
        eventBus.Start();

        // Act
        await eventBus.PublishAsync(testEvent);
        await Task.Delay(200); // Allow processing
        await eventBus.StopAsync();

        // Assert
        var processingTimes = metricsRegistry.GetHistogramValues(ProcessingTime.Name);
        Assert.NotEmpty(processingTimes);
        Assert.All(processingTimes, time => Assert.True(time >= 0));
    }
}
