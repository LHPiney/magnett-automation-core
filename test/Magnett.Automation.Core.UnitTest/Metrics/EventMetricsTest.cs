using System;
using System.Collections.Generic;
using Magnett.Automation.Core.Metrics;
using Magnett.Automation.Core.Events.Metrics;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Metrics;

public class EventMetricsTest
{
    [Fact]
    public void EventsPublished_ShouldHaveCorrectProperties()
    {
        var metric = new EventsPublished("TestEvent");
        
        Assert.Equal(MetricType.Counter, metric.Type);
        Assert.Equal("events.published", EventsPublished.Name);
        Assert.Equal("TestEvent", metric.Tags["event_type"]);
        Assert.Single(metric.Tags);
    }

    [Fact]
    public void EventsProcessed_ShouldHaveCorrectProperties()
    {
        var metric = new EventsProcessed("TestEvent");
        
        Assert.Equal(MetricType.Counter, metric.Type);
        Assert.Equal("events.processed", EventsProcessed.Name);
        Assert.Equal("TestEvent", metric.Tags["event_type"]);
        Assert.Single(metric.Tags);
    }

    [Fact]
    public void EventsFailed_ShouldHaveCorrectProperties()
    {
        var metric = new EventsFailed("TestEvent");
        
        Assert.Equal(MetricType.Counter, metric.Type);
        Assert.Equal("events.failed", EventsFailed.Name);
        Assert.Equal("TestEvent", metric.Tags["event_type"]);
        Assert.Single(metric.Tags);
    }

    [Fact]
    public void ProcessingTime_ShouldHaveCorrectProperties()
    {
        var metric = new ProcessingTime("TestEvent");
        
        Assert.Equal(MetricType.Histogram, metric.Type);
        Assert.Equal("processing.time", ProcessingTime.Name);
        Assert.Equal("TestEvent", metric.Tags["event_type"]);
        Assert.Single(metric.Tags);
    }

    [Fact]
    public void QueueSize_ShouldHaveCorrectProperties()
    {
        var metric = new QueueSize("eventbus");
        
        Assert.Equal(MetricType.Gauge, metric.Type);
        Assert.Equal("queue.size", QueueSize.Name);
        Assert.Equal("eventbus", metric.Tags["component"]);
        Assert.Single(metric.Tags);
    }

    [Fact]
    public void EventsPublished_WithDifferentEventTypes_ShouldBeDifferent()
    {
        var metric1 = new EventsPublished("TestEvent1");
        var metric2 = new EventsPublished("TestEvent2");
        
        Assert.NotEqual(metric1, metric2);
        Assert.Equal("TestEvent1", metric1.Tags["event_type"]);
        Assert.Equal("TestEvent2", metric2.Tags["event_type"]);
    }

    [Fact]
    public void ProcessingTime_WithDifferentEventTypes_ShouldBeDifferent()
    {
        var metric1 = new ProcessingTime("TestEvent1");
        var metric2 = new ProcessingTime("TestEvent2");
        
        Assert.NotEqual(metric1, metric2);
        Assert.Equal("TestEvent1", metric1.Tags["event_type"]);
        Assert.Equal("TestEvent2", metric2.Tags["event_type"]);
    }

    [Fact]
    public void QueueSize_WithDifferentComponents_ShouldBeDifferent()
    {
        var metric1 = new QueueSize("eventbus");
        var metric2 = new QueueSize("workflow");
        
        Assert.NotEqual(metric1, metric2);
        Assert.Equal("eventbus", metric1.Tags["component"]);
        Assert.Equal("workflow", metric2.Tags["component"]);
    }

    [Fact]
    public void EventsPublished_WithSameEventType_ShouldHaveSameContent()
    {
        var metric1 = new EventsPublished("TestEvent");
        var metric2 = new EventsPublished("TestEvent");
        
        Assert.Equal(metric1.Type, metric2.Type);
        Assert.Equal(metric1.Tags["event_type"], metric2.Tags["event_type"]);
        Assert.Equal(metric1.Tags.Count, metric2.Tags.Count);
    }

    [Fact]
    public void ProcessingTime_WithSameEventType_ShouldHaveSameContent()
    {
        var metric1 = new ProcessingTime("TestEvent");
        var metric2 = new ProcessingTime("TestEvent");
        
        Assert.Equal(metric1.Type, metric2.Type);
        Assert.Equal(metric1.Tags["event_type"], metric2.Tags["event_type"]);
        Assert.Equal(metric1.Tags.Count, metric2.Tags.Count);
    }

    [Fact]
    public void QueueSize_WithSameComponent_ShouldHaveSameContent()
    {
        var metric1 = new QueueSize("eventbus");
        var metric2 = new QueueSize("eventbus");
        
        Assert.Equal(metric1.Type, metric2.Type);
        Assert.Equal(metric1.Tags["component"], metric2.Tags["component"]);
        Assert.Equal(metric1.Tags.Count, metric2.Tags.Count);
    }

    [Fact]
    public void EventsPublished_ShouldImplementIMetric()
    {
        var metric = new EventsPublished("TestEvent");
        
        Assert.IsAssignableFrom<IMetric>(metric);
        Assert.Equal("events.published", ((IMetric)metric).Name);
    }

    [Fact]
    public void EventsProcessed_ShouldImplementIMetric()
    {
        var metric = new EventsProcessed("TestEvent");
        
        Assert.IsAssignableFrom<IMetric>(metric);
        Assert.Equal("events.processed", ((IMetric)metric).Name);
    }

    [Fact]
    public void EventsFailed_ShouldImplementIMetric()
    {
        var metric = new EventsFailed("TestEvent");
        
        Assert.IsAssignableFrom<IMetric>(metric);
        Assert.Equal("events.failed", ((IMetric)metric).Name);
    }

    [Fact]
    public void ProcessingTime_ShouldImplementIMetric()
    {
        var metric = new ProcessingTime("TestEvent");
        
        Assert.IsAssignableFrom<IMetric>(metric);
        Assert.Equal("processing.time", ((IMetric)metric).Name);
    }

    [Fact]
    public void QueueSize_ShouldImplementIMetric()
    {
        var metric = new QueueSize("eventbus");
        
        Assert.IsAssignableFrom<IMetric>(metric);
        Assert.Equal("queue.size", ((IMetric)metric).Name);
    }
}
