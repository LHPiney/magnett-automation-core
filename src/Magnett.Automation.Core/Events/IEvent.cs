namespace Magnett.Automation.Core.Events;

/// <summary>
/// Interface that represents an event, which is a message that is sent to notify subscribers about something that has happened.
/// Event record is a base class for all events.
/// <seealso cref="Event"/>
/// </summary>
public interface IEvent
{
    Guid Id { get; }
    string Name { get; }
    string Caller { get; }
    DateTime CreatedAt { get; }
    string ToString()
    {
        return $"Event: {Name} - Type: {GetType().Name} - CreatedAt: {CreatedAt}";
    }
}        
 

