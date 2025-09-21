using System;
using System.Collections.Generic;
using System.Linq;
using Magnett.Automation.Core.Metrics;
using Magnett.Automation.Core.Metrics.Implementations;
using Magnett.Automation.Core.Events.Metrics;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Metrics;

public class MetricsRegistryTest
{
    private readonly MetricsRegistry _registry;

    public MetricsRegistryTest()
    {
        _registry = new MetricsRegistry();
    }

    [Fact]
    public void Register_ShouldStoreMetric()
    {
        var metric = new EventsPublished("TestEvent");
        
        _registry.Register(metric);
        
        var retrievedMetric = _registry.GetMetric<EventsPublished>("events.published");
        Assert.Equal(metric, retrievedMetric);
    }

    [Fact]
    public void Register_ShouldUpdateExistingMetric()
    {
        var metric1 = new EventsPublished("TestEvent1");
        var metric2 = new EventsPublished("TestEvent2");
        
        _registry.Register(metric1);
        _registry.Register(metric2);
        
        var retrievedMetric = _registry.GetMetric<EventsPublished>("events.published");
        Assert.Equal(metric2, retrievedMetric);
    }

    [Fact]
    public void GetMetric_WithNonExistentName_ShouldThrowException()
    {
        Assert.Throws<InvalidOperationException>(() => 
            _registry.GetMetric<EventsPublished>("non.existent"));
    }

    [Fact]
    public void GetMetric_WithWrongType_ShouldThrowException()
    {
        var metric = new EventsPublished("TestEvent");
        _registry.Register(metric);
        
        Assert.Throws<InvalidOperationException>(() => 
            _registry.GetMetric<EventsProcessed>("events.published"));
    }

    [Fact]
    public void GetAllMetrics_ShouldReturnAllRegisteredMetrics()
    {
        var metric1 = new EventsPublished("TestEvent1");
        var metric2 = new EventsProcessed("TestEvent2");
        var metric3 = new ProcessingTime("TestEvent3");
        
        _registry.Register(metric1);
        _registry.Register(metric2);
        _registry.Register(metric3);
        
        var allMetrics = _registry.GetAllMetrics().ToList();
        
        Assert.Equal(3, allMetrics.Count);
        Assert.Contains(metric1, allMetrics);
        Assert.Contains(metric2, allMetrics);
        Assert.Contains(metric3, allMetrics);
    }

    [Fact]
    public void GetMetricsByType_ShouldReturnOnlyMatchingType()
    {
        var counter1 = new EventsPublished("TestEvent1");
        var counter2 = new EventsProcessed("TestEvent2");
        var histogram = new ProcessingTime("TestEvent3");
        
        _registry.Register(counter1);
        _registry.Register(counter2);
        _registry.Register(histogram);
        
        var counters = _registry.GetMetricsByType(MetricType.Counter).ToList();
        
        Assert.Equal(2, counters.Count);
        Assert.Contains(counter1, counters);
        Assert.Contains(counter2, counters);
        Assert.DoesNotContain(histogram, counters);
    }

    [Fact]
    public void GetMetricsByTag_ShouldReturnMatchingMetrics()
    {
        var metric1 = new EventsPublished("TestEvent1");
        var metric2 = new EventsProcessed("TestEvent2");
        var metric3 = new QueueSize("other");
        
        _registry.Register(metric1);
        _registry.Register(metric2);
        _registry.Register(metric3);
        
        var eventTypeMetrics = _registry.GetMetricsByTag("event_type", "TestEvent1").ToList();
        
        Assert.Single(eventTypeMetrics);
        Assert.Equal(EventsPublished.Name, eventTypeMetrics.First().Name);
        Assert.Equal("TestEvent1", eventTypeMetrics.First().Tags["event_type"]);
    }

    [Fact]
    public void UpdateCounter_ShouldIncrementValue()
    {
        var metric = new EventsPublished("TestEvent");
        _registry.Register(metric);
        
        _registry.UpdateCounter("events.published", 5);
        _registry.UpdateCounter("events.published", 3);
        
        Assert.Equal(8, _registry.GetCounterValue("events.published"));
    }

    [Fact]
    public void SetCounter_ShouldSetExactValue()
    {
        var metric = new EventsPublished("TestEvent");
        _registry.Register(metric);
        
        _registry.SetCounter("events.published", 10);
        _registry.SetCounter("events.published", 5);
        
        Assert.Equal(5, _registry.GetCounterValue("events.published"));
    }

    [Fact]
    public void AddHistogramValue_ShouldStoreValues()
    {
        var metric = new ProcessingTime("TestEvent");
        _registry.Register(metric);
        
        _registry.AddHistogramValue("processing.time", 10.5);
        _registry.AddHistogramValue("processing.time", 15.2);
        _registry.AddHistogramValue("processing.time", 12.8);
        
        var values = _registry.GetHistogramValues("processing.time");
        
        Assert.Equal(3, values.Count);
        Assert.Contains(10.5, values);
        Assert.Contains(15.2, values);
        Assert.Contains(12.8, values);
    }

    [Fact]
    public void GetHistogramAverage_ShouldCalculateCorrectAverage()
    {
        var metric = new ProcessingTime("TestEvent");
        _registry.Register(metric);
        
        _registry.AddHistogramValue("processing.time", 10.0);
        _registry.AddHistogramValue("processing.time", 20.0);
        _registry.AddHistogramValue("processing.time", 30.0);
        
        Assert.Equal(20.0, _registry.GetHistogramAverage("processing.time"));
    }

    [Fact]
    public void GetHistogramMinMax_ShouldReturnCorrectValues()
    {
        var metric = new ProcessingTime("TestEvent");
        _registry.Register(metric);
        
        _registry.AddHistogramValue("processing.time", 15.0);
        _registry.AddHistogramValue("processing.time", 5.0);
        _registry.AddHistogramValue("processing.time", 25.0);
        
        Assert.Equal(5.0, _registry.GetHistogramMin("processing.time"));
        Assert.Equal(25.0, _registry.GetHistogramMax("processing.time"));
    }

    [Fact]
    public void SetGauge_ShouldStoreValue()
    {
        var metric = new QueueSize("eventbus");
        _registry.Register(metric);
        
        _registry.SetGauge("queue.size", 42.5);
        
        Assert.Equal(42.5, _registry.GetGaugeValue("queue.size"));
    }

    [Fact]
    public void GetCounterValue_WithNonExistentCounter_ShouldReturnZero()
    {
        Assert.Equal(0, _registry.GetCounterValue("non.existent"));
    }

    [Fact]
    public void GetGaugeValue_WithNonExistentGauge_ShouldReturnZero()
    {
        Assert.Equal(0, _registry.GetGaugeValue("non.existent"));
    }

    [Fact]
    public void GetHistogramValues_WithNonExistentHistogram_ShouldReturnEmptyList()
    {
        var values = _registry.GetHistogramValues("non.existent");
        
        Assert.Empty(values);
    }

    [Fact]
    public void GetHistogramAverage_WithEmptyHistogram_ShouldReturnZero()
    {
        Assert.Equal(0, _registry.GetHistogramAverage("non.existent"));
    }

    [Fact]
    public void GetHistogramMinMax_WithEmptyHistogram_ShouldReturnZero()
    {
        Assert.Equal(0, _registry.GetHistogramMin("non.existent"));
        Assert.Equal(0, _registry.GetHistogramMax("non.existent"));
    }
}
