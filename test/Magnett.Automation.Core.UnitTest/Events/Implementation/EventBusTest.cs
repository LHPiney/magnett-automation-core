using System;
using System.Threading.Tasks;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.UnitTest.Events.Fakes;
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
        await using var eventBus = EventBus.Create(logger.Object);

        // Assert
        Assert.NotNull(eventBus);
    }

    [Fact]
    public async Task GetEventReader_ShouldReturnEventReader()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();
        await using var eventBus =  EventBus.Create(logger.Object);

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
        Assert.Throws<ArgumentNullException>(() => EventBus.Create(logger!));
    }

    [Fact]
    public void PublishAsync_ShouldThrowArgumentNullException_WhenEventIsNull()
    {
        // Arrange
        var logger = new Mock<ILogger<EventBus>>();
        var eventBus = EventBus.Create(logger.Object);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => eventBus.PublishAsync(null));
    }
}