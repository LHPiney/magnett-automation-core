namespace Magnett.Automation.Core.Metrics;

public interface IMetricsCollector
{
    void Record<T>(T metric, double value) where T : IMetric;
    void Increment<T>(T metric) where T : IMetric;
    void Set<T>(T metric, double value) where T : IMetric;
}
