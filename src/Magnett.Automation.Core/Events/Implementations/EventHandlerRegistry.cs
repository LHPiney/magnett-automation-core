using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.Events.Implementations;

/// <summary>
/// Implementation of event handler registry with fluent API for registration and instance caching
/// </summary>
public sealed class EventHandlerRegistry : IEventHandlerRegistry, IDisposable
{
    private readonly HashSet<Type> _eventHandlers = [];
    private readonly MemoryCache _handlerCache;
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private bool _disposed;
    
    private EventHandlerRegistry(ILogger logger, IServiceProvider serviceProvider = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        _logger = logger;
        _handlerCache = new MemoryCache(new MemoryCacheOptions());
        _serviceProvider = serviceProvider;
    }
    
    public IReadOnlyList<Type> GetEventHandlersTypes<TEvent>(TEvent @event)
      where TEvent : IEvent
    {
        return [.. _eventHandlers
            .Where(handler => handler.GetInterfaces()
            .Any(@interface => @interface.IsGenericType && 
                               @interface.GetGenericTypeDefinition() == typeof(IEventHandler<>) &&
                               @interface.GetGenericArguments().First() == @event.GetType()))];
    }

    
    public IReadOnlyList<IEventHandler<TEvent>> GetEventHandlers<TEvent>(TEvent @event)
      where TEvent : IEvent
    {
        ArgumentNullException.ThrowIfNull(@event);
        
        var handlerTypes = GetEventHandlersTypes(@event);
        var handlers = new List<IEventHandler<TEvent>>();
        foreach (var handlerType in handlerTypes)
        {
            try
            {
                var handler = CreateOrGetCachedHandler<TEvent>(handlerType);
                if (handler != null)
                {
                    handlers.Add(handler);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to create handler instance for type {HandlerType}", handlerType.Name);
            }
        }
        
        return handlers;
    }
    
    public IEventHandlerRegistry FromAssembly(string assemblyName)
    {
        if (string.IsNullOrWhiteSpace(assemblyName))
            throw new ArgumentException("Assembly name cannot be null or whitespace", nameof(assemblyName));
        
        try
        {
            var assembly = Assembly.Load(assemblyName);
            var handlers = FindEventHandlersInAssembly(assembly);
            
            foreach (var handler in handlers.ToList())
            {
                _eventHandlers.Add(handler);
                _logger?.LogDebug("Registered event handler {HandlerType} from assembly {Assembly}", 
                    handler.Name, assemblyName);
            }
            
            _logger?.LogInformation("Registered {Count} event handlers from assembly {Assembly}", 
                handlers.Count(), assemblyName);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to load assembly {Assembly} for event handler registration", assemblyName);
            throw;
        }
        
        return this;
    }
    
    public IEventHandlerRegistry FromNamespace(string @namespace)
    {
        if (string.IsNullOrWhiteSpace(@namespace))
            throw new ArgumentException("Namespace cannot be null or whitespace", nameof(@namespace));
        
        var handlers = FindEventHandlersInNamespace(@namespace);
        
        foreach (var handler in handlers)
        {
            _eventHandlers.Add(handler);
            _logger?.LogDebug("Registered event handler {HandlerType} from namespace {Namespace}", 
                handler.Name, @namespace);
        }
        
        _logger?.LogInformation("Registered {Count} event handlers from namespace {Namespace}", 
            handlers.Count(), @namespace);
        
        return this;
    }
    
    public IEventHandlerRegistry Register<THandler>() where THandler : class
    {
        var handlerType = typeof(THandler);
        return Register(handlerType);
    }
    
    public IEventHandlerRegistry Register(Type handlerType)
    {
        ArgumentNullException.ThrowIfNull(handlerType);
        
        if (!IsValidEventHandler(handlerType))
        {
            throw new ArgumentException($"Type {handlerType.Name} is not a valid event handler. " +
                "It must implement IEventHandler<T> interface.", nameof(handlerType));
        }
        
        _eventHandlers.Add(handlerType);
        _logger?.LogDebug("Registered event handler {HandlerType}", handlerType.Name);
        
        return this;
    }
    
    public IEventHandlerRegistry RegisterMany(IEnumerable<Type> handlerTypes)
    {
        ArgumentNullException.ThrowIfNull(handlerTypes);
        
        foreach (var handlerType in handlerTypes)
        {
            Register(handlerType);
        }
        
        return this;
    }
    
    public IEventHandlerRegistry Clear()
    {
        var count = _eventHandlers.Count;
        _eventHandlers.Clear();
        _handlerCache.Clear();
        _logger.LogInformation("Cleared {Count} registered event handlers and cache", count);
        
        return this;
    }
    
    private object CreateWithActivator(Type handlerType)
    {
        var constructors = handlerType.GetConstructors();
        if (constructors.Length == 0)
            throw new InvalidOperationException($"No public constructors found for {handlerType.Name}");

        var constructor = constructors
            .OrderByDescending(c => c.GetParameters().Length)
            .First();

        var parameters = constructor.GetParameters();
        var args = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var paramType = parameters[i].ParameterType;
            object value = null;

            if (_serviceProvider != null)
            {
                if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(ILogger<>))
                {
                    var loggerFactory = _serviceProvider.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
                    if (loggerFactory != null)
                    {
                        var loggerType = paramType.GenericTypeArguments[0];
                        value = typeof(LoggerFactoryExtensions)
                            .GetMethod("CreateLogger", [typeof(ILoggerFactory)])?
                            .MakeGenericMethod(loggerType)
                            .Invoke(null, [loggerFactory]);
                    }
                }
                else
                {
                    value = _serviceProvider.GetService(paramType);
                }
            }
            args[i] = value;
        }

        return Activator.CreateInstance(handlerType, args);
    }
    
    private IEventHandler<TEvent> CreateOrGetCachedHandler<TEvent>(Type handlerType)
      where TEvent : IEvent
    {
        var key = $"handler:{handlerType.FullName}";
    
        if (_handlerCache.TryGetValue(key, out var cachedHandler))
        {
            return cachedHandler as IEventHandler<TEvent>;
        }
        
        try
        {
            object handler = null;
            if (_serviceProvider != null)
            {
                handler = _serviceProvider.GetService(handlerType);
            }
            if (handler == null)
            {
                handler = CreateWithActivator(handlerType);
            }
            _handlerCache.Set(key, handler);
            _logger?.LogDebug("Created and cached handler instance for {HandlerType}", handlerType.Name);
            
            return handler as IEventHandler<TEvent>;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to create handler instance for {HandlerType}", handlerType.Name);
            return null;
        }
    }
    
    private static IEnumerable<Type> FindEventHandlersInAssembly(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(IsValidEventHandler);
    }
    
    private static IEnumerable<Type> FindEventHandlersInNamespace(string @namespace)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.Namespace?.StartsWith(@namespace, StringComparison.OrdinalIgnoreCase) == true)
            .Where(IsValidEventHandler);
    }
    
    private static bool IsValidEventHandler(Type type)
    {
        return type.IsClass &&
               !type.IsAbstract && 
               type.GetInterfaces()
                   .Any(@interface => @interface.IsGenericType && 
                                      @interface.GetGenericTypeDefinition() == typeof(IEventHandler<>));
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        Clear();
        _disposed = true;
        _handlerCache?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Creates a new EventHandlerRegistry instance
    /// </summary>
    public static EventHandlerRegistry Create(ILogger logger = null, IServiceProvider serviceProvider = null)
    {
        return new EventHandlerRegistry(logger, serviceProvider);
    }

    /// <summary>
    /// Creates a new EventHandlerRegistry with automatic discovery of all event handlers
    /// </summary>
    public static EventHandlerRegistry CreateWithAutoDiscovery(ILogger logger = null, IServiceProvider serviceProvider = null)
    {
        var registry = new EventHandlerRegistry(logger, serviceProvider);
        
        var allHandlers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(IsValidEventHandler);
        
        registry.RegisterMany(allHandlers);
        
        return registry;
    }
} 