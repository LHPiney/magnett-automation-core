using System;
using System.Collections.Generic;
using Magnett.Automation.Core.Metrics;
using Magnett.Automation.Core.Metrics.Implementations;
using Magnett.Automation.Core.Events.Metrics;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Metrics;

public class MetricsCollectorTest
{
    private readonly MetricsRegistry _registry;
    private readonly MetricsCollector _collector;

    public MetricsCollectorTest()
    {
        _registry = new MetricsRegistry();
        _collector = MetricsCollector.Create(_registry);
    }

    [Fact]
    public void Record_WithCounterMetric_ShouldIncrementCounter()
    {
        var metric = new EventsPublished("TestEvent");
        
        _collector.Record(metric, 5);
        _collector.Record(metric, 3);
        
        Assert.Equal(8, _registry.GetCounterValue("events.published"));
    }

    [Fact]
    public void Record_WithHistogramMetric_ShouldAddValue()
    {
        var metric = new ProcessingTime("TestEvent");
        
        _collector.Record(metric, 10.5);
        _collector.Record(metric, 15.2);
        
        var values = _registry.GetHistogramValues("processing.time");
        Assert.Equal(2, values.Count);
        Assert.Contains(10.5, values);
        Assert.Contains(15.2, values);
    }

    [Fact]
    public void Record_WithGaugeMetric_ShouldSetValue()
    {
        var metric = new QueueSize("eventbus");
        
        _collector.Record(metric, 42.5);
        _collector.Record(metric, 30.0);
        
        Assert.Equal(30.0, _registry.GetGaugeValue("queue.size"));
    }

    [Fact]
    public void Increment_WithCounterMetric_ShouldIncrementByOne()
    {
        var metric = new EventsPublished("TestEvent");
        
        _collector.Increment(metric);
        _collector.Increment(metric);
        _collector.Increment(metric);
        
        Assert.Equal(3, _registry.GetCounterValue("events.published"));
    }

    [Fact]
    public void Increment_WithNonCounterMetric_ShouldNotChangeValue()
    {
        var metric = new ProcessingTime("TestEvent");
        
        _collector.Increment(metric);
        
        Assert.Equal(0, _registry.GetHistogramValues("processing.time").Count);
    }

    [Fact]
    public void Set_WithGaugeMetric_ShouldSetValue()
    {
        var metric = new QueueSize("eventbus");
        
        _collector.Set(metric, 100.0);
        
        Assert.Equal(100.0, _registry.GetGaugeValue("queue.size"));
    }

    [Fact]
    public void Set_WithNonGaugeMetric_ShouldNotChangeValue()
    {
        var metric = new EventsPublished("TestEvent");
        
        _collector.Set(metric, 100.0);
        
        Assert.Equal(0, _registry.GetCounterValue("events.published"));
    }

    [Fact]
    public void Record_ShouldRegisterMetric()
    {
        var metric = new EventsPublished("TestEvent");
        
        _collector.Record(metric, 1);
        
        var retrievedMetric = _registry.GetMetric<EventsPublished>("events.published");
        Assert.Equal(metric, retrievedMetric);
    }

    [Fact]
    public void Increment_ShouldRegisterMetric()
    {
        var metric = new EventsProcessed("TestEvent");
        
        _collector.Increment(metric);
        
        var retrievedMetric = _registry.GetMetric<EventsProcessed>("events.processed");
        Assert.Equal(metric, retrievedMetric);
    }

    [Fact]
    public void Set_ShouldRegisterMetric()
    {
        var metric = new QueueSize("eventbus");
        
        _collector.Set(metric, 50.0);
        
        var retrievedMetric = _registry.GetMetric<QueueSize>("queue.size");
        Assert.Equal(metric, retrievedMetric);
    }

    [Fact]
    public void Record_WithZeroValue_ShouldStillRecord()
    {
        var metric = new EventsPublished("TestEvent");
        
        _collector.Record(metric, 0);
        
        Assert.Equal(0, _registry.GetCounterValue("events.published"));
    }

    [Fact]
    public void Record_WithNegativeValue_ShouldRecordNegativeValue()
    {
        var metric = new EventsPublished("TestEvent");
        
        _collector.Record(metric, -5);
        
        Assert.Equal(-5, _registry.GetCounterValue("events.published"));
    }
}
