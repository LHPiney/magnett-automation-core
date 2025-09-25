namespace Magnett.Automation.Core.Events.Implementations;

/// <summary>
/// Represents the state of the event bus.
/// </summary>
public record EventBusState : Enumeration
{
    public static readonly EventBusState Ready   = new(1, nameof(Ready));
    public static readonly EventBusState Running = new(2, nameof(Running));
    public static readonly EventBusState Stopped = new(3, nameof(Stopped));

    private EventBusState(int id, string name) : base(id, name)
    {
    }
}