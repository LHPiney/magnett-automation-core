using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Channels;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.UnitTest.Events.Fakes;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Events.Implementation;

public class EventStreamTest
{
    [Fact]
    public void Create_WithLogger_ShouldReturnInstance()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStream>>();

        // Act
        var stream = EventStream.Create(logger.Object);

        // Assert
        Assert.NotNull(stream);
        Assert.IsAssignableFrom<IEventStream>(stream);
    }

    [Fact]
    public void Create_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => EventStream.Create(null!));
    }

    [Fact]
    public void Reader_ShouldReturnChannelReader()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStream>>();
        var stream = EventStream.Create(logger.Object);

        // Act
        var reader = stream.Reader;

        // Assert
        Assert.NotNull(reader);
    }

    [Fact]
    public void Writer_ShouldReturnChannelWriter()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStream>>();
        var stream = EventStream.Create(logger.Object);

        // Act
        var writer = stream.Writer;

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public async Task Writer_WriteAsync_ShouldWriteEvent()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStream>>();
        await using var stream = EventStream.Create(logger.Object);
        var testEvent = new TestEvent("test_data");

        // Act
        await stream.Writer.WriteAsync(testEvent);

        // Assert
        // Event was written successfully (no exception thrown)
        Assert.True(true);
    }

    [Fact]
    public async Task Reader_ReadAsync_ShouldReadEvent()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStream>>();
        await using var stream = EventStream.Create(logger.Object);
        var testEvent = new TestEvent("test_data");

        // Act
        await stream.Writer.WriteAsync(testEvent);
        var result = await stream.Reader.ReadAsync();

        // Assert
        Assert.Equal(testEvent, result);
    }

    [Fact]
    public async Task DisposeAsync_ShouldCompleteWriter()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStream>>();
        var stream = EventStream.Create(logger.Object);

        // Act
        await stream.DisposeAsync();

        // Assert
        // Writer should be completed - TryComplete returns false if already completed
        var result = stream.Writer.TryComplete();
        Assert.False(result); // Should be false because it's already completed
    }

    [Fact]
    public async Task MultipleWrites_ShouldHandleMultipleEvents()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStream>>();
        await using var stream = EventStream.Create(logger.Object);
        var events = new[]
        {
            new TestEvent("event1"),
            new TestEvent("event2"),
            new TestEvent("event3")
        };

        // Act
        foreach (var evt in events)
        {
            await stream.Writer.WriteAsync(evt);
        }

        // Assert
        for (int i = 0; i < events.Length; i++)
        {
            var result = await stream.Reader.ReadAsync();
            Assert.Equal(events[i], result);
        }
    }

    [Fact]
    public void Create_WithOptions_ShouldUseOptions()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStream>>();
        var options = new EventStreamOptions
        {
            MaxBufferSize = 10,
            AllowMultipleReaders = true,
            AllowMultipleWriters = false,
            FullMode = BoundedChannelFullMode.Wait
        };

        // Act
        var stream = EventStream.Create(logger.Object, options);

        // Assert
        Assert.NotNull(stream);
        Assert.IsAssignableFrom<IEventStream>(stream);
    }
}
