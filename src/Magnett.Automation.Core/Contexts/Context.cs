#nullable enable

using System.Linq;
using System.Threading;
using Magnett.Automation.Core.Contexts.Events;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.Contexts;

/// <summary>
///  Represents a context that can store and retrieve values of heterogeneous types.
///  The data is stored in an <seealso cref="IContextVault"/> IContextVault.
///  If not provided, new <seealso cref="ContextVault"/> will be created as the default implementation.
///  If no event bus is provided, the context will not be able to publish events.
///  Otherwise, it will use the provided event bus.
///
/// <param name="contextVault">Implementation of data vault store</param>
/// <param name="eventBus">Eventbus to publish context's events</param>
/// </summary>
public class Context : EventEmitterEntity
{
    public Guid Id { get; }
    public DateTime CreatedAt { get; }

    private readonly IContextVault _contextVault;
    private readonly object _syncRoot = new();

    public IEventReader? EventStream => EventBus?.EventReader;

    private Context(IContextVault contextVault, IEventBus? eventBus) 
        : base(eventBus)
    {
        _contextVault = contextVault ?? throw new ArgumentNullException(nameof(contextVault));

        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
    
    public async Task<Context> StoreAsync<TValue>(
        ContextField<TValue> field, 
        TValue value, 
        CancellationToken cancellationToken = default)
    {
        TValue? previousValue;
        bool shouldEmitEvent = false;

        lock (_syncRoot)
        {
            previousValue = Value(field);
            _contextVault.Set(field, value);

            shouldEmitEvent = !(previousValue is null && value is null) &&
                              !(previousValue is not null && previousValue.Equals(value));
        }

        if (shouldEmitEvent)
        {
            await EmitEventAsync(
                OnChangeFieldValueEvent.Create(field.Name, typeof(TValue), value, previousValue),
                cancellationToken);
        }

        return this;
    }

    public TValue? Value<TValue>(ContextField<TValue> field)
    {
        if (!_contextVault.HasItem(field)) return default(TValue);;

        return (TValue) _contextVault.Get(field);
    }

    public bool TryGetValue<TValue>(ContextField<TValue> field, out TValue value)
    {
        lock (_syncRoot)
        {
            if (_contextVault.HasItem(field))
            {
                value = (TValue)_contextVault.Get(field);
                return true;
            }

            value = default!;
            return false;
        }
    }

    public static Context Create(IContextVault contextVault, IEventBus? eventBus = null)
    {
        return new Context(contextVault, eventBus);
    }
        
    public static Context Create(IEventBus? eventBus = null)
    {
        return new Context(ContextVault.Create(), eventBus);
    }

}