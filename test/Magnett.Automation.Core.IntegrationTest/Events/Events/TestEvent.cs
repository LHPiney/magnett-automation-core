using System;
using Magnett.Automation.Core.Events;

namespace Magnett.Automation.Core.IntegrationTest.Events.Events;

public record TestEvent(string Caller) : IEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; } = nameof(TestEvent);
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}