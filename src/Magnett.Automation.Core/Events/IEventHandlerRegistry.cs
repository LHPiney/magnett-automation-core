namespace Magnett.Automation.Core.Events;

/// <summary>
/// Registry for event handlers that provides flexible registration options
/// </summary>
public interface IEventHandlerRegistry
{
    /// <summary>
    /// Gets all registered event handler types for handle a specific event type
    /// </summary>
    IReadOnlyList<Type> GetEventHandlersTypes<TEvent>(TEvent @event)
    where TEvent : IEvent;
    
    /// <summary>
    /// Creates instances of handlers for a specific event
    /// </summary>
    /// <param name="event">Event source</param>
    /// <returns>Collection of handler instances</returns>
    IReadOnlyList<IEventHandler<TEvent>> GetEventHandlers<TEvent>(TEvent @event) 
    where TEvent : IEvent;
    
    /// <summary>
    /// Registers event handlers from a specific assembly
    /// </summary>
    /// <param name="assemblyName">Name of the assembly to scan</param>
    /// <returns>Self for fluent API</returns>
    IEventHandlerRegistry FromAssembly(string assemblyName);
    
    /// <summary>
    /// Registers event handlers from a specific namespace
    /// </summary>
    /// <param name="namespace">Namespace to scan for handlers</param>
    /// <returns>Self for fluent API</returns>
    IEventHandlerRegistry FromNamespace(string @namespace);
    
    /// <summary>
    /// Registers a specific event handler type
    /// </summary>
    /// <typeparam name="THandler">Type of the event handler</typeparam>
    /// <returns>Self for fluent API</returns>
    IEventHandlerRegistry Register<THandler>() where THandler : class;
    
    /// <summary>
    /// Registers a specific event handler type by Type
    /// </summary>
    /// <param name="handlerType">Type of the event handler</param>
    /// <returns>Self for fluent API</returns>
    IEventHandlerRegistry Register(Type handlerType);
    
    /// <summary>
    /// Registers multiple event handler types
    /// </summary>
    /// <param name="handlerTypes">Collection of handler types</param>
    /// <returns>Self for fluent API</returns>
    IEventHandlerRegistry RegisterMany(IEnumerable<Type> handlerTypes);
    
    /// <summary>
    /// Clears all registered handlers and cache
    /// </summary>
    /// <returns>Self for fluent API</returns>
    IEventHandlerRegistry Clear();
} 