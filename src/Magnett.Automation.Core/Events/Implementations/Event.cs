namespace Magnett.Automation.Core.Events.Implementations;

/// <summary>
/// Represents a core abstract base record for all events. The Event class includes key details
/// such as a unique identifier, event name, and the caller who originates the event.
/// It serves as a base type for specific event implementations.
/// </summary>
/// <remarks>
/// This record is immutable and defines essential properties such as:
/// Id - A unique identifier for the event instance.
/// Name - The name of the event, provided as an input.
/// Caller - The identifier or context of the initiator of the event.
/// CreatedAt - The UTC timestamp indicating when the event was created.
/// </remarks>
public abstract record Event(string Name, string Caller) : IEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; } = Name ?? throw new ArgumentNullException(nameof(Name));
    public string Caller { get; } = Caller ?? string.Empty;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}