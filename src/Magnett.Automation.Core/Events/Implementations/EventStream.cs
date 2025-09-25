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
public sealed class EventStream : IEventStream
{
    private readonly Channel<IEvent> _channel;
    private readonly ILogger _logger;
    private bool _disposed;

    public ChannelReader<IEvent> Reader => _channel.Reader;
    public ChannelWriter<IEvent> Writer => _channel.Writer;

    private EventStream(ILogger logger, EventStreamOptions options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _channel = CreateChannelFromOptions(options);
    }

    private static Channel<IEvent> CreateChannelFromOptions(EventStreamOptions options)
    {
        if (options.MaxBufferSize is <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options.MaxBufferSize), "MaxBufferSize must be greater than zero.");
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

    public ValueTask DisposeAsync()
    {
        if (_disposed) return ValueTask.CompletedTask;

        try
        {
            _disposed = true;
            _channel.Writer.TryComplete();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error disposing event stream");
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Creates a new EventStream instance with the provided logger and options.
    /// </summary>
    /// <param name="logger">The logger to use for event stream operations.</param>
    /// <param name="options">Optional configuration options for the event stream.</param>
    /// <returns>A new EventStream instance.</returns>
    public static IEventStream Create(ILogger logger, EventStreamOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        return new EventStream(logger, options ?? new EventStreamOptions());
    }
}