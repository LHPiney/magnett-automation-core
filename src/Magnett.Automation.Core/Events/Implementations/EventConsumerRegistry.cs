using System.Collections.Concurrent;
using System.Linq;

namespace Magnett.Automation.Core.Events.Implementations;

public class EventConsumerRegistry : IEventConsumerRegistry
{
    private readonly ConcurrentDictionary<Guid, IEventConsumer> _consumers = new();

    public void RegisterConsumer(IEventConsumer consumer)
    {
        ArgumentNullException.ThrowIfNull(consumer);

        if (!_consumers.TryAdd(consumer.ConsumerId, consumer))
        {
            throw new InvalidOperationException($"Consumer with ID {consumer.ConsumerId} is already registered");
        }
    }

    public void UnregisterConsumer(Guid consumerId)
    {
        if (consumerId == Guid.Empty) throw new ArgumentException("Consumer ID cannot be null or empty");
        
        _consumers.TryRemove(consumerId, out _);
    }

    public IEnumerable<IEventConsumer> GetConsumers()
    {
        return _consumers.Values;
    }

    public IEnumerable<IEventConsumer> GetConsumers(Type consumerType)
    {
        return _consumers
            .Values
            .Where(c => c.GetType() == consumerType);
    }
} 