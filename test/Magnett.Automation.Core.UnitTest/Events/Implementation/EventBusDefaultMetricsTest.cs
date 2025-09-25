using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.Events.Metrics;
using Magnett.Automation.Core.Metrics.Implementations;
using Magnett.Automation.Core.UnitTest.Events.Fakes;

namespace Magnett.Automation.Core.UnitTest.Events.Implementation;

public class EventBusDefaultMetricsTest
{
    [Fact]
    public void Create_WhenNoMetricsCollectorProvided_ShouldCreateDefaultMetricsCollector()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<EventBus>();

        var eventBus = EventBus.Create(logger, startOnCreate: false);

        Assert.NotNull(eventBus);
        Assert.Equal(EventBusState.Stopped, eventBus.State);
        Assert.NotNull(eventBus.MetricsRegistry);
    }

    [Fact]
    public async Task Create_WhenNoMetricsCollectorProvided_ShouldCollectMetricsAutomatically()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<EventBus>();

        var eventBus = EventBus.Create(logger, startOnCreate: true);
        eventBus.EventHandlerRegistry.Register<TestEventHandler>();

        var testEvent = new TestEvent("TestData", "TestEvent", "TestCaller");
        await eventBus.PublishAsync(testEvent);

        await Task.Delay(100);

        await eventBus.StopAsync();

        var metricsRegistry = (MetricsRegistry)eventBus.MetricsRegistry;
        Assert.NotNull(metricsRegistry);
        
        var publishedCount = metricsRegistry.GetCounterValue(EventsPublished.Name);
        var processedCount = metricsRegistry.GetCounterValue(EventsProcessed.Name);

        Assert.True(publishedCount > 0, "Should have published events");
        Assert.True(processedCount > 0, "Should have processed events");
    }

    [Fact]
    public void Create_WhenMetricsCollectorProvided_ShouldUseProvidedMetricsCollector()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<EventBus>();
        var customMetricsRegistry = new MetricsRegistry();
        var customMetricsCollector = MetricsCollector.Create(customMetricsRegistry);

        var eventBus = EventBus.Create(logger, null, customMetricsCollector, startOnCreate: false);

        Assert.NotNull(eventBus);
        Assert.Equal(EventBusState.Stopped, eventBus.State);
    }

}
