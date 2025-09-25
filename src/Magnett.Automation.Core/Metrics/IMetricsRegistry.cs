namespace Magnett.Automation.Core.Metrics;

public interface IMetricsRegistry
{
    void Register<T>(T metric) where T : IMetric;
    T GetMetric<T>(string name) where T : IMetric;
    IEnumerable<IMetric> GetAllMetrics();
    IEnumerable<IMetric> GetMetricsByType(MetricType type);
    IEnumerable<IMetric> GetMetricsByTag(string key, string value);
}
