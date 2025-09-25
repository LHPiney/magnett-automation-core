using System;
using Magnett.Automation.Core.Events;

namespace Magnett.Automation.Core.UnitTest.Events.Fakes;

/// <summary>
/// Evento de prueba para usar en tests unitarios
/// </summary>
public record TestEvent(string Data, string Name = "TestEvent", string Caller = "TestCaller") : IEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}

/// <summary>
/// Otro tipo de evento para probar múltiples tipos
/// </summary>
public record AnotherTestEvent(
    string Name = "AnotherTestEvent",
    string Caller = "TestCaller",
    string AdditionalData = "")
    : IEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
} 