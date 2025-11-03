using Magnett.Automation.Core.Events;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.Contexts.Serializations;

public interface ISerializationContext
{
    IEventBus? EventBus { get; }
    ILogger? Logger { get; }
    IServiceProvider? ServiceProvider { get; }
}

