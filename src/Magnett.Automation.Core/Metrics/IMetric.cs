namespace Magnett.Automation.Core.Metrics;

public interface IMetric
{
    MetricType Type { get; }
    string Name { get; }
    IReadOnlyDictionary<string, string> Tags { get; }
}

public sealed record MetricType(int Id, string Name) : Enumeration(Id, Name)
{
    public static readonly MetricType Counter = new(1, "Counter");
    public static readonly MetricType Histogram = new(2, "Histogram");
    public static readonly MetricType Gauge = new(3, "Gauge");
}
