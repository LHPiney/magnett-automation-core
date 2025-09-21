using System.Collections.Generic;
using Magnett.Automation.Core.Metrics;

namespace Magnett.Automation.Core.Events.Metrics;

public sealed record EventsProcessed : IMetric
{
    public const string Name = "events.processed";
    
    public MetricType Type => MetricType.Counter;
    string IMetric.Name => Name;
    public IReadOnlyDictionary<string, string> Tags { get; }
    
    public EventsProcessed(string eventType)
    {
        Tags = new Dictionary<string, string> { ["event_type"] = eventType };
    }
}
