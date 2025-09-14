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
        await using var eventStream = new EventStream(_streamLogger);

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
}