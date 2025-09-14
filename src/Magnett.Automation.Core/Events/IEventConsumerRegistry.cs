using System;

namespace Magnett.Automation.Core.Events;

public interface IEventConsumerRegistry
{
    void RegisterConsumer(IEventConsumer consumer);
    void UnregisterConsumer(Guid consumerId);
    IEnumerable<IEventConsumer> GetConsumers();
    IEnumerable<IEventConsumer> GetConsumers(Type consumerType);
} 