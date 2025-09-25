using System;
using Magnett.Automation.Core.Events;

namespace Magnett.Automation.Core.UnitTest.Events.Fakes;

/// <summary>
/// Evento de prueba para usar en tests unitarios
/// </summary>
public record SecondTestEvent(string Name = "TestEvent", string Caller = "TestCaller") : IEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}