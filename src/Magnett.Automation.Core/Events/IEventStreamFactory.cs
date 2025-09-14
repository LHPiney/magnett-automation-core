using Magnett.Automation.Core.Events.Implementations;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.Events;

public interface IEventStreamFactory
{
    IEventStream CreateStream(ILogger logger);
    IEventStream CreateStream(ILogger logger, EventStreamOptions options);
} 