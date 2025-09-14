using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.Events.Implementations;

public class EventStreamFactory : IEventStreamFactory
{
    public IEventStream CreateStream(ILogger logger)
    {
        return new EventStream(logger, new EventStreamOptions
        {
            AllowMultipleReaders = true,
            AllowMultipleWriters = false,
            FullMode = BoundedChannelFullMode.Wait,
        });
    }

    public IEventStream CreateStream(ILogger logger, EventStreamOptions options)
    {
        return new EventStream(logger, options);
    }
} 