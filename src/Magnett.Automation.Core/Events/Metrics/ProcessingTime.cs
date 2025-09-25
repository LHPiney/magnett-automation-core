using System.Collections.Generic;
using Magnett.Automation.Core.Metrics;

namespace Magnett.Automation.Core.Events.Metrics;

public sealed record ProcessingTime : IMetric
{
    public const string Name = "processing.time";
    
    public MetricType Type => MetricType.Histogram;
    string IMetric.Name => Name;
    public IReadOnlyDictionary<string, string> Tags { get; }
    
    public ProcessingTime(string eventType)
    {
        Tags = new Dictionary<string, string> { ["event_type"] = eventType };
    }
}
