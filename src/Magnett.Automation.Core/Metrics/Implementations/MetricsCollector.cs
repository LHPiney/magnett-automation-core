using Magnett.Automation.Core.Metrics;

namespace Magnett.Automation.Core.Metrics.Implementations;

public class MetricsCollector : IMetricsCollector
{
    private readonly MetricsRegistry _registry;

    public IMetricsRegistry Registry => _registry;
    
    private MetricsCollector(IMetricsRegistry registry)
    {
        _registry = (MetricsRegistry)(registry ?? throw new ArgumentNullException(nameof(registry)));
    }
    
    public void Record<T>(T metric, double value) where T : IMetric
    {
        _registry.Register(metric);
        
        switch (metric.Type.Name)
        {
            case "Counter":
                _registry.UpdateCounter(metric.Name, (long)value);
                break;
            case "Histogram":
                _registry.AddHistogramValue(metric.Name, value);
                break;
            case "Gauge":
                _registry.SetGauge(metric.Name, value);
                break;
        }
    }
    
    public void Increment<T>(T metric) where T : IMetric
    {
        _registry.Register(metric);
        
        if (metric.Type.Name == "Counter")
        {
            _registry.UpdateCounter(metric.Name, 1);
        }
    }
    
    public void Set<T>(T metric, double value) where T : IMetric
    {
        _registry.Register(metric);
        
        if (metric.Type.Name == "Gauge")
        {
            _registry.SetGauge(metric.Name, value);
        }
    }

    /// <summary>
    /// Creates a new MetricsCollector instance with a default MetricsRegistry.
    /// </summary>
    /// <returns>A new MetricsCollector instance.</returns>
    public static MetricsCollector Create()
    {
        var registry = new MetricsRegistry();
        return new MetricsCollector(registry);
    }

    /// <summary>
    /// Creates a new MetricsCollector instance with the provided MetricsRegistry.
    /// </summary>
    /// <param name="registry">The metrics registry to use.</param>
    /// <returns>A new MetricsCollector instance.</returns>
    public static MetricsCollector Create(IMetricsRegistry registry)
    {
        return new MetricsCollector(registry);
    }
}
