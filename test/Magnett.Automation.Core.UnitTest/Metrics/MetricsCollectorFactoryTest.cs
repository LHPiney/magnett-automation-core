using System.Threading.Tasks;
using Xunit;
using Magnett.Automation.Core.Metrics;
using Magnett.Automation.Core.Metrics.Implementations;
using Magnett.Automation.Core.Events.Metrics;

namespace Magnett.Automation.Core.UnitTest.Metrics;

public class MetricsCollectorFactoryTest
{
    [Fact]
    public void Create_ShouldCreateMetricsCollectorWithDefaultRegistry()
    {
        var metricsCollector = MetricsCollector.Create();

        Assert.NotNull(metricsCollector);
        Assert.NotNull(metricsCollector.Registry);
    }

    [Fact]
    public void Create_ShouldAllowRecordingMetrics()
    {
        var metricsCollector = MetricsCollector.Create();

        metricsCollector.Increment(new EventsPublished("TestEvent"));
        metricsCollector.Record(new ProcessingTime("TestEvent"), 100.5);
        metricsCollector.Set(new QueueSize("test"), 5.0);

        var registry = (MetricsRegistry)metricsCollector.Registry;
        var publishedCount = registry.GetCounterValue(EventsPublished.Name);
        var processingTimes = registry.GetHistogramValues(ProcessingTime.Name);
        var queueSize = registry.GetGaugeValue(QueueSize.Name);

        Assert.Equal(1, publishedCount);
        Assert.Single(processingTimes);
        Assert.Equal(100.5, processingTimes[0]);
        Assert.Equal(5.0, queueSize);
    }

    [Fact]
    public void Create_ShouldCreateIndependentInstances()
    {
        var collector1 = MetricsCollector.Create();
        var collector2 = MetricsCollector.Create();

        collector1.Increment(new EventsPublished("Event1"));
        collector2.Increment(new EventsPublished("Event2"));

        var registry1 = (MetricsRegistry)collector1.Registry;
        var registry2 = (MetricsRegistry)collector2.Registry;
        
        var count1 = registry1.GetCounterValue(EventsPublished.Name);
        var count2 = registry2.GetCounterValue(EventsPublished.Name);

        Assert.Equal(1, count1);
        Assert.Equal(1, count2);
    }
}
