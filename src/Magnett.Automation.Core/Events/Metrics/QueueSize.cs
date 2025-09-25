using System.Collections.Generic;
using Magnett.Automation.Core.Metrics;

namespace Magnett.Automation.Core.Events.Metrics;

public sealed record QueueSize : IMetric
{
    public const string Name = "queue.size";
    
    public MetricType Type => MetricType.Gauge;
    string IMetric.Name => Name;
    public IReadOnlyDictionary<string, string> Tags { get; }
    
    public QueueSize(string component)
    {
        Tags = new Dictionary<string, string> { ["component"] = component };
    }
}
