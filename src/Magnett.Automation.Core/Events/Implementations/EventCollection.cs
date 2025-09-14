namespace Magnett.Automation.Core.Events.Implementations;

/// <summary>
/// Class that stores a collection of events. This class is a wrapper around a list of IEvent.
/// Used by EventSourceEntities <seealso cref="EventSourceEntity"/>
/// </summary>
public class EventCollection : List<IEvent>;