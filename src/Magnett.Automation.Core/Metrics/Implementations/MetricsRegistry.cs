using System.Collections.Concurrent;
using System.Linq;

namespace Magnett.Automation.Core.Metrics.Implementations;

public class MetricsRegistry : IMetricsRegistry, IMetricsViewer
{
    private readonly ConcurrentDictionary<string, IMetric> _metrics = new();
    private readonly ConcurrentDictionary<string, long> _counters = new();
    private readonly ConcurrentDictionary<string, ConcurrentBag<double>> _histograms = new();
    private readonly ConcurrentDictionary<string, double> _gauges = new();
    
    public void Register<T>(T metric) where T : IMetric
    {
        _metrics.AddOrUpdate(metric.Name, metric, (_, _) => metric);
    }
    
    internal void UpdateCounter(string name, long value)
    {
        _counters.AddOrUpdate(name, value, (_, current) => current + value);
    }
    
    internal void SetCounter(string name, long value)
    {
        _counters.AddOrUpdate(name, value, (_, _) => value);
    }
    
    internal void AddHistogramValue(string name, double value)
    {
        _histograms.AddOrUpdate(name, 
            new ConcurrentBag<double> { value }, 
            (_, bag) => { bag.Add(value); return bag; });
    }
    
    internal void SetGauge(string name, double value)
    {
        _gauges.AddOrUpdate(name, value, (_, _) => value);
    }
    
    public T GetMetric<T>(string name) where T : IMetric
    {
        if (_metrics.TryGetValue(name, out var metric) && metric is T typedMetric)
        {
            return typedMetric;
        }
        throw new InvalidOperationException($"Metric '{name}' not found or type mismatch");
    }
    
    public IEnumerable<IMetric> GetAllMetrics()
    {
        return _metrics.Values;
    }
    
    public IEnumerable<IMetric> GetMetricsByType(MetricType type)
    {
        return _metrics.Values.Where(m => m.Type == type);
    }
    
    public IEnumerable<IMetric> GetMetricsByTag(string key, string value)
    {
        return _metrics.Values.Where(m => 
            m.Tags.TryGetValue(key, out var tagValue) && tagValue == value);
    }
    
    public long GetCounterValue(string name)
    {
        return _counters.TryGetValue(name, out var value) ? value : 0;
    }
    
    public double GetGaugeValue(string name)
    {
        return _gauges.TryGetValue(name, out var value) ? value : 0;
    }
    
    public IReadOnlyList<double> GetHistogramValues(string name)
    {
        return _histograms.TryGetValue(name, out var values) ? values.ToList().AsReadOnly() : new List<double>().AsReadOnly();
    }
    
    public double GetHistogramAverage(string name)
    {
        var values = GetHistogramValues(name);
        return values.Count > 0 ? values.Average() : 0;
    }
    
    public double GetHistogramMin(string name)
    {
        var values = GetHistogramValues(name);
        return values.Count > 0 ? values.Min() : 0;
    }
    
    public double GetHistogramMax(string name)
    {
        var values = GetHistogramValues(name);
        return values.Count > 0 ? values.Max() : 0;
    }
}
