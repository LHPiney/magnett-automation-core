using System.Collections.Generic;
using Magnett.Automation.Core.Metrics;

namespace Magnett.Automation.Core.Events.Metrics;

public sealed record EventsPublished : IMetric
{
    public const string Name = "events.published";
    
    public MetricType Type => MetricType.Counter;
    string IMetric.Name => Name;
    public IReadOnlyDictionary<string, string> Tags { get; }
    
    public EventsPublished(string eventType)
    {
        Tags = new Dictionary<string, string> { ["event_type"] = eventType };
    }
}
