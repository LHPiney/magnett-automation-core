using System.Threading.Channels;

namespace Magnett.Automation.Core.Events.Implementations;

public record EventStreamOptions
{
    public int? MaxBufferSize { get; init; }
    public bool AllowMultipleReaders { get; init; } = false;
    public bool AllowMultipleWriters { get; init; } = true;
    public BoundedChannelFullMode FullMode { get; init; } = BoundedChannelFullMode.Wait;
} 