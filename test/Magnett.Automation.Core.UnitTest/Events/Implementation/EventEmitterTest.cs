using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.UnitTest.Events.Fakes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Events.Implementation;

public class EventEmitterTest
{
    private readonly ILogger<EventBus> _busLogger;
    private readonly ILogger<EventStream> _streamLogger;

    public EventEmitterTest()
    {
        var loggerFactory = new NullLoggerFactory();
        _busLogger = loggerFactory.CreateLogger<EventBus>();
        _streamLogger = loggerFactory.CreateLogger<EventStream>();
    }

    [Fact]
    public async Task EmitEventAsync_WhenBusAndStreamAreProvided_ShouldPublishToBoth()
    {
        // Arrange
        await using var eventBus = EventBus.Create(_busLogger);
        await using var eventStream = EventStream.Create(_streamLogger);

        var testEntity = new TestEntity(eventBus);
        const string eventData = "test_data_123";

        // Act
        eventBus.Start();
        await testEntity.DoSomethingAsync(eventData);

        // Assert
        var localEvent = Assert.Single(testEntity.LocalEvents);
        Assert.IsType<TestEvent>(localEvent);
        Assert.Equal(eventData, ((TestEvent)localEvent).Data);
        
        var streamEvent = default(IEvent);
        if (await eventBus.EventReader.Reader.WaitToReadAsync())
        {
            if (eventBus.EventReader.Reader.TryRead(out var item))
            {
               streamEvent = item;
            }
        }
        await eventBus.StopAsync();
        Assert.Equal(localEvent, streamEvent);
    }

    [Fact]
    public async Task EmitEventAsync_WhenCancellationTokenIsRequested_ShouldReturnEarly()
    {
        // Arrange
        await using var eventBus = EventBus.Create(_busLogger);
        var testEntity = new TestEntity(eventBus);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        eventBus.Start();
        await testEntity.DoSomethingAsync("test_data", cts.Token);

        // Assert
        Assert.Empty(testEntity.LocalEvents);
        await eventBus.StopAsync();
    }

    [Fact]
    public async Task EmitEventAsync_WhenEventBusIsNull_ShouldOnlyStoreLocally()
    {
        // Arrange
        var testEntity = new TestEntity(null);
        const string eventData = "test_data_null_bus";

        // Act
        await testEntity.DoSomethingAsync(eventData);

        // Assert
        var localEvent = Assert.Single(testEntity.LocalEvents);
        Assert.IsType<TestEvent>(localEvent);
        Assert.Equal(eventData, ((TestEvent)localEvent).Data);
    }

    [Fact]
    public async Task EmitEventAsync_WithMultipleEvents_ShouldOrderByCreatedAt()
    {
        // Arrange
        await using var eventBus = EventBus.Create(_busLogger);
        var testEntity = new TestEntity(eventBus);

        // Act
        eventBus.Start();
        await testEntity.DoSomethingAsync("event1");
        await Task.Delay(10); // Ensure different timestamps
        await testEntity.DoSomethingAsync("event2");
        await Task.Delay(10);
        await testEntity.DoSomethingAsync("event3");

        // Assert
        var events = testEntity.LocalEvents.ToList();
        Assert.Equal(3, events.Count);
        
        for (int i = 1; i < events.Count; i++)
        {
            Assert.True(events[i-1].CreatedAt <= events[i].CreatedAt);
        }
        
        await eventBus.StopAsync();
    }

    [Fact]
    public void ClearLocalEvents_ShouldRemoveAllStoredEvents()
    {
        // Arrange
        var testEntity = new TestEntity(null);
        
        // Act
        testEntity.DoSomethingAsync("event1").Wait();
        testEntity.DoSomethingAsync("event2").Wait();
        Assert.Equal(2, testEntity.LocalEvents.Count);
        
        testEntity.ClearLocalEvents();

        // Assert
        Assert.Empty(testEntity.LocalEvents);
    }

    [Fact]
    public async Task EmitEventAsync_WithNullEvent_ShouldThrowArgumentNullException()
    {
        // Arrange
        var testEntity = new TestEntity(null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => testEntity.EmitNullEventAsync());
    }
}