using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.Metrics.Implementations;
using Magnett.Automation.Core.IntegrationTest.Events.Events;
using Magnett.Automation.Core.Events.Metrics;

namespace Magnett.Automation.Core.IntegrationTest.Events;

public class EventTest
{
    private const int DefaultEventCount = 100; 
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(1);

    [Fact]
    public async Task Run_WhenInvoke_ProcessAllEvents()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<EventTest>();

        //Setup Event System
        var (eventBus, eventStream, metricsRegistry) = await SetupEventSystem(loggerFactory);
        
        // Execute
        var results = await ProcessEventsInParallel(eventBus, eventStream, metricsRegistry, logger);
        
        // Validate
        await ValidateResults(results, metricsRegistry, logger, DefaultEventCount);
    }

    private static Task<(IEventBus eventBus, IEventStream eventStream, MetricsRegistry metricsRegistry)> SetupEventSystem(ILoggerFactory loggerFactory)
    {
        var busLogger = loggerFactory.CreateLogger<EventBus>();
        var streamLogger = loggerFactory.CreateLogger<EventStream>();

        var metricsRegistry = new MetricsRegistry();
        var metricsCollector = MetricsCollector.Create(metricsRegistry);

        var eventStream = EventStream.Create(streamLogger);
        var eventBus = EventBus.Create(busLogger, eventStream, metricsCollector);
        eventBus.EventHandlerRegistry.FromAssembly("Magnett.Automation.Core.IntegrationTest");

        return Task.FromResult((eventBus, eventStream, metricsRegistry));
    }

    private async Task<ProcessingResults> ProcessEventsInParallel(
        IEventBus eventBus, 
        IEventStream eventStream, 
        MetricsRegistry metricsRegistry, 
        ILogger<EventTest> logger)
    {
        var streamedCount = 0;
        var cancellationTokenSource = new CancellationTokenSource(DefaultTimeout);
                     
        //Start Event Stream consumer 
        var streamConsumerTask = StartEventStreamConsumer(
            eventStream, 
            logger, 
            () => streamedCount, 
            count => streamedCount = count, 
            cancellationTokenSource.Token);

        //Start a Realtime metrics process 
        var metricsTask = StartMetricsMonitor(metricsRegistry, logger, cancellationTokenSource.Token);

        logger.LogInformation("Publishing {DefaultEventCount:N0} events in parallel...", DefaultEventCount);
        var publishStartTime = DateTime.UtcNow;
        var publishTasks = Enumerable.Range(0, DefaultEventCount).Select(async i =>
        {
            var @event = new TestEvent("Caller " + i);
            await eventBus.PublishAsync(@event);
            
            if (i % DefaultEventCount == 0)
            {
                logger.LogInformation("Published {I} events...", i + 1);
            }
        });
        
        await Task.WhenAll(publishTasks);
        var publishEndTime = DateTime.UtcNow;

        await WaitForProcessingCompletion(metricsRegistry, logger, DefaultEventCount, cancellationTokenSource.Token);
        
        await eventBus.StopAsync();
        cancellationTokenSource.Cancel();
        
        try
        {
            await streamConsumerTask;
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("EventStream consumer cancelled gracefully");
        }
        
        try
        {
            await metricsTask;
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Metrics monitor cancelled gracefully");
        }

        return new ProcessingResults
        {
            StreamedCount = streamedCount,
            PublishStartTime = publishStartTime,
            PublishEndTime = publishEndTime
        };
    }

    private static Task StartEventStreamConsumer(
        IEventStream eventStream, 
        ILogger<EventTest> logger, 
        Func<int> getCount, 
        Action<int> setCount, 
        CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            try
            {
                await foreach (var streamedEvent in eventStream.Reader.ReadAllAsync(cancellationToken))
                {
                    setCount(getCount() + 1);
                }

                logger.LogInformation("EventStream: {GetCount} events persisted", getCount());
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("EventStream: {GetCount} events persisted (cancelled)", getCount());
            }
            catch (Exception ex)
            {
                logger.LogInformation("Error reading from EventStream: {ExMessage}", ex.Message);
            }
        }, cancellationToken);
    }

    private static Task StartMetricsMonitor(
        MetricsRegistry metricsRegistry, 
        ILogger<EventTest> logger, 
        CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(500, cancellationToken);
                
                try
                {
                    var publishedCount = metricsRegistry.GetCounterValue(EventsPublished.Name);
                    var processedCount = metricsRegistry.GetCounterValue(EventsProcessed.Name);
                    var failedCount = metricsRegistry.GetCounterValue(EventsFailed.Name);
                    var queueSize = metricsRegistry.GetGaugeValue(QueueSize.Name);
                    var avgProcessingTime = metricsRegistry.GetHistogramAverage(ProcessingTime.Name);
                    
                    logger.LogInformation("METRICS: Published={PublishedCount}, Processed={ProcessedCount}, Failed={FailedCount}, Queue={QueueSize}, AvgTime={AvgProcessingTime:F2}ms", publishedCount, processedCount, failedCount, queueSize, avgProcessingTime);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogInformation("Error reading metrics: {ExMessage}", ex.Message);
                }
            }
        }, cancellationToken);
    }

    private static async Task WaitForProcessingCompletion(
        MetricsRegistry metricsRegistry, 
        ILogger<EventTest> logger, 
        int eventCount, 
        CancellationToken cancellationToken)
    {
        var timeout = TimeSpan.FromSeconds(30);
        var startTime = DateTime.UtcNow;
        
        while (metricsRegistry.GetCounterValue(EventsProcessed.Name) < eventCount)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (DateTime.UtcNow - startTime > timeout)
            {
                logger.LogWarning("Timeout reached after {TimeoutTotalSeconds}s. Processed: {GetCounterValue}/{EventCount}", timeout.TotalSeconds, metricsRegistry.GetCounterValue(EventsProcessed.Name), eventCount);
                break;
            }
            
            await Task.Delay(100, cancellationToken);
            var processed = metricsRegistry.GetCounterValue(EventsProcessed.Name);
            var published = metricsRegistry.GetCounterValue(EventsPublished.Name);
            
            if (processed % 50 == 0)
            {
                logger.LogInformation("Progress: {Processed}/{Published} events processed ({D:F1}%)", processed, published, processed * 100.0 / published);
            }
        }
    }

    private static Task ValidateResults(
        ProcessingResults results, 
        MetricsRegistry metricsRegistry, 
        ILogger<EventTest> logger, 
        int expectedEventCount)
    {
        logger.LogInformation("\nMETRICS SUMMARY:");
        
        try
        {
            var publishedCount = metricsRegistry.GetCounterValue(EventsPublished.Name);
            var processedCount = metricsRegistry.GetCounterValue(EventsProcessed.Name);
            var failedCount = metricsRegistry.GetCounterValue(EventsFailed.Name);
            var queueSize = metricsRegistry.GetGaugeValue(QueueSize.Name);
            var avgProcessingTime = metricsRegistry.GetHistogramAverage(ProcessingTime.Name);
            var minProcessingTime = metricsRegistry.GetHistogramMin(ProcessingTime.Name);
            var maxProcessingTime = metricsRegistry.GetHistogramMax(ProcessingTime.Name);
            var processingTimeValues = metricsRegistry.GetHistogramValues(ProcessingTime.Name);
            
            // Performance metrics
            var totalPublishTime = results.PublishEndTime - results.PublishStartTime;
            var eventsPerSecond = expectedEventCount / totalPublishTime.TotalSeconds;
            
            logger.LogInformation("   Events Published (Counter): {PublishedCount:N0}", publishedCount);
            logger.LogInformation("   Events Processed (Counter): {ProcessedCount:N0}", processedCount);
            logger.LogInformation("   Events Failed (Counter): {FailedCount:N0}", failedCount);
            logger.LogInformation("   Queue Size (Gauge): {QueueSize:N0}", queueSize);
            logger.LogInformation($"   Processing Time (Histogram):");
            logger.LogInformation("     Average: {AvgProcessingTime:F2}ms", avgProcessingTime);
            logger.LogInformation("     Min: {MinProcessingTime:F2}ms", minProcessingTime);
            logger.LogInformation("     Max: {MaxProcessingTime:F2}ms", maxProcessingTime);
            logger.LogInformation("     Samples: {Count:N0}", processingTimeValues.Count);
            logger.LogInformation($"   Performance:");
            logger.LogInformation("     Publish Rate: {EventsPerSecond:F0} events/sec", eventsPerSecond);
            logger.LogInformation("     Total Publish Time: {TotalMilliseconds:F0}ms", totalPublishTime.TotalMilliseconds);
            
            if (processedCount > 0)
            {
                var successRate = (double)processedCount / publishedCount * 100;
                logger.LogInformation("   Success Rate: {SuccessRate:F1}%", successRate);
            }

            // Assertions
            Assert.Equal(expectedEventCount, processedCount);
            Assert.Equal(expectedEventCount, publishedCount);
            Assert.Equal(0, failedCount);
            Assert.Equal(expectedEventCount, results.StreamedCount);
            Assert.True(eventsPerSecond > 50, $"Publish rate too low: {eventsPerSecond:F0} events/sec");
            Assert.True(totalPublishTime.TotalSeconds < 30, $"Publish time too long: {totalPublishTime.TotalSeconds:F1}s");
        }
        catch (Exception ex)
        {
            logger.LogError("   Error reading metrics: {ExMessage}", ex.Message);
            throw;
        }
        
        return Task.CompletedTask;
    }

    private class ProcessingResults
    {
        public int StreamedCount { get; set; }
        public DateTime PublishStartTime { get; set; }
        public DateTime PublishEndTime { get; set; }
    }
}