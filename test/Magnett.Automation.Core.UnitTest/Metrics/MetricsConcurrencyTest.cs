using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magnett.Automation.Core.Metrics;
using Magnett.Automation.Core.Metrics.Implementations;
using Magnett.Automation.Core.Events.Metrics;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Metrics;

public class MetricsConcurrencyTest
{
    private readonly MetricsRegistry _registry;
    private readonly MetricsCollector _collector;

    public MetricsConcurrencyTest()
    {
        _registry = new MetricsRegistry();
        _collector = MetricsCollector.Create(_registry);
    }

    [Fact]
    public async Task ConcurrentCounterUpdates_ShouldBeThreadSafe()
    {
        const int numberOfTasks = 10;
        const int incrementsPerTask = 100;
        var metric = new EventsPublished("TestEvent");
        
        var tasks = Enumerable.Range(0, numberOfTasks).Select(async _ =>
        {
            for (int i = 0; i < incrementsPerTask; i++)
            {
                _collector.Increment(metric);
                await Task.Yield();
            }
        });

        await Task.WhenAll(tasks);

        var expectedValue = numberOfTasks * incrementsPerTask;
        Assert.Equal(expectedValue, _registry.GetCounterValue("events.published"));
    }

    [Fact]
    public async Task ConcurrentHistogramUpdates_ShouldBeThreadSafe()
    {
        const int numberOfTasks = 10;
        const int valuesPerTask = 100;
        var metric = new ProcessingTime("TestEvent");
        
        var tasks = Enumerable.Range(0, numberOfTasks).Select(async taskId =>
        {
            for (int i = 0; i < valuesPerTask; i++)
            {
                var value = taskId * valuesPerTask + i;
                _collector.Record(metric, value);
                await Task.Yield();
            }
        });

        await Task.WhenAll(tasks);

        var values = _registry.GetHistogramValues("processing.time");
        Assert.Equal(numberOfTasks * valuesPerTask, values.Count);
        
        var expectedSum = Enumerable.Range(0, numberOfTasks * valuesPerTask).Sum();
        var actualSum = values.Sum();
        Assert.Equal(expectedSum, actualSum);
    }

    [Fact]
    public async Task ConcurrentGaugeUpdates_ShouldBeThreadSafe()
    {
        const int numberOfTasks = 10;
        var metric = new QueueSize("eventbus");
        
        var tasks = Enumerable.Range(0, numberOfTasks).Select(async taskId =>
        {
            for (int i = 0; i < 100; i++)
            {
                var value = taskId * 100 + i;
                _collector.Set(metric, value);
                await Task.Yield();
            }
        });

        await Task.WhenAll(tasks);

        var finalValue = _registry.GetGaugeValue("queue.size");
        Assert.True(finalValue >= 0 && finalValue < numberOfTasks * 100);
    }

    [Fact]
    public async Task ConcurrentMetricRegistration_ShouldBeThreadSafe()
    {
        const int numberOfTasks = 10;
        
        var tasks = Enumerable.Range(0, numberOfTasks).Select(async taskId =>
        {
            var metric = new EventsPublished($"TestEvent{taskId}");
            _collector.Increment(metric);
            await Task.Yield();
        });

        await Task.WhenAll(tasks);

        var allMetrics = _registry.GetAllMetrics().ToList();
        Assert.Single(allMetrics); // Solo una métrica porque todas tienen el mismo nombre
        
        var counterValue = _registry.GetCounterValue("events.published");
        Assert.Equal(numberOfTasks, counterValue);
    }

    [Fact]
    public async Task ConcurrentReadWriteOperations_ShouldBeThreadSafe()
    {
        const int numberOfTasks = 10;
        var metric = new EventsPublished("TestEvent");
        var readResults = new ConcurrentBag<long>();
        
        var writeTasks = Enumerable.Range(0, numberOfTasks).Select(async _ =>
        {
            for (int i = 0; i < 50; i++)
            {
                _collector.Increment(metric);
                await Task.Yield();
            }
        });

        var readTasks = Enumerable.Range(0, numberOfTasks).Select(async _ =>
        {
            for (int i = 0; i < 50; i++)
            {
                var value = _registry.GetCounterValue("events.published");
                readResults.Add(value);
                await Task.Yield();
            }
        });

        await Task.WhenAll(writeTasks.Concat(readTasks));

        var finalValue = _registry.GetCounterValue("events.published");
        Assert.Equal(numberOfTasks * 50, finalValue);
        
        Assert.Equal(numberOfTasks * 50, readResults.Count);
        Assert.All(readResults, value => Assert.True(value >= 0 && value <= numberOfTasks * 50));
    }

    [Fact]
    public async Task ConcurrentHistogramCalculations_ShouldBeThreadSafe()
    {
        const int numberOfTasks = 5;
        const int valuesPerTask = 100;
        var metric = new ProcessingTime("TestEvent");
        
        var tasks = Enumerable.Range(0, numberOfTasks).Select(async taskId =>
        {
            for (int i = 0; i < valuesPerTask; i++)
            {
                var value = taskId * valuesPerTask + i;
                _collector.Record(metric, value);
                await Task.Yield();
            }
        });

        await Task.WhenAll(tasks);

        var values = _registry.GetHistogramValues("processing.time");
        var average = _registry.GetHistogramAverage("processing.time");
        var min = _registry.GetHistogramMin("processing.time");
        var max = _registry.GetHistogramMax("processing.time");

        Assert.Equal(numberOfTasks * valuesPerTask, values.Count);
        Assert.Equal(0, min);
        Assert.Equal((numberOfTasks - 1) * valuesPerTask + valuesPerTask - 1, max);
        
        var expectedAverage = values.Average();
        Assert.Equal(expectedAverage, average);
    }

    [Fact]
    public async Task ConcurrentMetricQueries_ShouldBeThreadSafe()
    {
        const int numberOfTasks = 10;
        var metrics = new List<IMetric>
        {
            new EventsPublished("TestEvent1"),
            new EventsProcessed("TestEvent2"),
            new EventsFailed("TestEvent3"),
            new ProcessingTime("TestEvent4"),
            new QueueSize("eventbus")
        };

        foreach (var metric in metrics)
        {
            _collector.Record(metric, 1);
        }

        var tasks = Enumerable.Range(0, numberOfTasks).Select(async _ =>
        {
            for (int i = 0; i < 100; i++)
            {
                var allMetrics = _registry.GetAllMetrics().ToList();
                var counterMetrics = _registry.GetMetricsByType(MetricType.Counter).ToList();
                var histogramMetrics = _registry.GetMetricsByType(MetricType.Histogram).ToList();
                var gaugeMetrics = _registry.GetMetricsByType(MetricType.Gauge).ToList();
                
                Assert.Equal(5, allMetrics.Count);
                Assert.Equal(3, counterMetrics.Count);
                Assert.Single(histogramMetrics);
                Assert.Single(gaugeMetrics);
                
                await Task.Yield();
            }
        });

        await Task.WhenAll(tasks);
    }
}
