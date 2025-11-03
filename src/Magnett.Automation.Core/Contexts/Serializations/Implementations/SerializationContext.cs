using Magnett.Automation.Core.Events;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.Contexts.Serializations.Implementations;

internal class SerializationContext : ISerializationContext
{
    public IEventBus? EventBus { get; }
    public ILogger? Logger { get; }
    public IServiceProvider? ServiceProvider { get; }

    public SerializationContext(
        IEventBus? eventBus = null, 
        ILogger? logger = null,
        IServiceProvider? serviceProvider = null)
    {
        EventBus = eventBus;
        Logger = logger;
        ServiceProvider = serviceProvider;
    }
}

