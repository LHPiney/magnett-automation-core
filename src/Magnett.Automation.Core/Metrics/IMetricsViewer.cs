namespace Magnett.Automation.Core.Metrics;

public interface IMetricsViewer
{
    T GetMetric<T>(string name) where T : IMetric;
    IEnumerable<IMetric> GetAllMetrics();
    IEnumerable<IMetric> GetMetricsByType(MetricType type);
    IEnumerable<IMetric> GetMetricsByTag(string key, string value);
    
    long GetCounterValue(string name);
    double GetGaugeValue(string name);
    IReadOnlyList<double> GetHistogramValues(string name);
    double GetHistogramAverage(string name);
    double GetHistogramMin(string name);
    double GetHistogramMax(string name);
}
