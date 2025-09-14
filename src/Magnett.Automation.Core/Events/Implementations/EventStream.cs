using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.Events.Implementations;

/// <summary>
/// Represents an event stream implementation using a channel
/// to handle concurrent event writing and reading operations.
/// </summary>
/// <remarks>
/// This class provides a mechanism for managing an unbounded channel of events
/// with dedicated readers and writers.
/// </remarks>
public sealed class EventStream(ILogger logger, EventStreamOptions? options = null)
    : IEventStream
{
    private readonly Channel<IEvent> _channel = CreateChannelFromOptions(options ?? new EventStreamOptions());
    private readonly ILogger _logger = logger;
    private bool _disposed;

    public ChannelReader<IEvent> Reader => _channel.Reader;
    public ChannelWriter<IEvent> Writer => _channel.Writer;
    public Task Completion => _channel.Reader.Completion;

    private static Channel<IEvent> CreateChannelFromOptions(EventStreamOptions options)
    {
        if (options.MaxBufferSize is <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options.MaxBufferSize), "MaxBufferSize debe ser mayor que cero.");
        }

        if (options.MaxBufferSize.HasValue)
        {
            var boundedOptions = new BoundedChannelOptions(options.MaxBufferSize.Value)
            {
                FullMode = options.FullMode
            };
            ConfigureCommonOptions(boundedOptions);
            return Channel.CreateBounded<IEvent>(boundedOptions);
        }

        var unboundedOptions = new UnboundedChannelOptions();
        ConfigureCommonOptions(unboundedOptions);
        return Channel.CreateUnbounded<IEvent>(unboundedOptions);

        void ConfigureCommonOptions(ChannelOptions channelOpts)
        {
            channelOpts.AllowSynchronousContinuations = false;
            channelOpts.SingleReader = !options.AllowMultipleReaders;
            channelOpts.SingleWriter = !options.AllowMultipleWriters;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        try
        {
            _disposed = true;
            _channel.Writer.Complete();
            await Completion;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error disposing event stream");
        }
    }
}