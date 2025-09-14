using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.UnitTest.Events.Fakes;

namespace Magnett.Automation.Core.UnitTest.Events.Implementation;

/// <summary>
/// Tests unitarios para EventHandlerRegistry
/// </summary>
public class EventHandlerRegistryTest : IDisposable
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly EventHandlerRegistry _registry;
    private readonly TestEvent _testEvent;
    private readonly AnotherTestEvent _anotherTestEvent;

    public EventHandlerRegistryTest()
    {
        _mockLogger = new Mock<ILogger>();
        _registry = EventHandlerRegistry.Create(_mockLogger.Object);
        _testEvent = new TestEvent("TestEvent", "TestCaller");
        _anotherTestEvent = new AnotherTestEvent("AnotherTestEvent", "TestCaller", "TestData");
    }

    public void Dispose()
    {
        _registry?.Dispose();
    }

    #region Constructor Tests

    [Fact]
    public void Create_WithLogger_ShouldCreateInstance()
    {
        // Act
        var registry = EventHandlerRegistry.Create(_mockLogger.Object);

        // Assert
        Assert.NotNull(registry);
        Assert.IsType<EventHandlerRegistry>(registry);
    }

    [Fact]
    public void Create_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => EventHandlerRegistry.Create(null));
    }

    [Fact]
    public void CreateWithAutoDiscovery_ShouldRegisterAllHandlers()
    {
        // Act
        var registry = EventHandlerRegistry.CreateWithAutoDiscovery(_mockLogger.Object);

        // Assert
        var handlers = registry.GetEventHandlersTypes(_testEvent);
        Assert.Contains(typeof(TestEventHandler), handlers);
        Assert.Contains(typeof(AnotherTestEventHandler), handlers);
    }

    #endregion

    #region Register Tests

    [Fact]
    public void Register_WithValidHandlerType_ShouldRegisterHandler()
    {
        // Act
        _registry.Register<TestEventHandler>();

        // Assert
        var handlers = _registry.GetEventHandlersTypes(_testEvent);
        Assert.Single(handlers);
        Assert.Contains(typeof(TestEventHandler), handlers);
    }

    [Fact]
    public void Register_WithValidHandlerTypeByType_ShouldRegisterHandler()
    {
        // Act
        _registry.Register(typeof(TestEventHandler));

        // Assert
        var handlers = _registry.GetEventHandlersTypes(_testEvent);
        Assert.Single(handlers);
        Assert.Contains(typeof(TestEventHandler), handlers);
    }

    [Fact]
    public void Register_WithNullType_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _registry.Register((Type)null));
    }

    [Fact]
    public void Register_WithInvalidHandlerType_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _registry.Register(typeof(InvalidHandler)));
        Assert.Contains("is not a valid event handler", exception.Message);
    }

    [Fact]
    public void Register_WithAbstractHandlerType_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _registry.Register(typeof(AbstractTestEventHandler)));
        Assert.Contains("is not a valid event handler", exception.Message);
    }

    [Fact]
    public void Register_WithDuplicateHandler_ShouldNotThrowException()
    {
        // Arrange
        _registry.Register<TestEventHandler>();

        // Act & Assert
        var exception = Record.Exception(() => _registry.Register<TestEventHandler>());
        Assert.Null(exception);
    }

    [Fact]
    public void RegisterMany_WithValidHandlers_ShouldRegisterAllHandlers()
    {
        // Arrange
        var handlerTypes = new[] { typeof(TestEventHandler), typeof(AnotherTestEventHandler) };

        // Act
        _registry.RegisterMany(handlerTypes);

        // Assert
        var handlers = _registry.GetEventHandlersTypes(_testEvent);
        Assert.Equal(2, handlers.Count);
        Assert.Contains(typeof(TestEventHandler), handlers);
        Assert.Contains(typeof(AnotherTestEventHandler), handlers);
    }

    [Fact]
    public void RegisterMany_WithNullCollection_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _registry.RegisterMany(null));
    }

    [Fact]
    public void RegisterMany_WithMixedValidAndInvalidHandlers_ShouldThrowArgumentException()
    {
        // Arrange
        var handlerTypes = new[] { typeof(TestEventHandler), typeof(InvalidHandler) };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _registry.RegisterMany(handlerTypes));
    }

    #endregion

    #region GetEventHandlers Tests

    [Fact]
    public void GetEventHandlers_WithNoRegisteredHandlers_ShouldReturnEmptyList()
    {
        // Act
        var handlers = _registry.GetEventHandlersTypes(_testEvent);

        // Assert
        Assert.Empty(handlers);
    }

    [Fact]
    public void GetEventHandlers_WithRegisteredHandlers_ShouldReturnCorrectHandlers()
    {
        // Arrange
        _registry.Register<TestEventHandler>();
        _registry.Register<AnotherTestEventHandler>();

        // Act
        var handlers = _registry.GetEventHandlersTypes(_testEvent);

        // Assert
        Assert.Equal(2, handlers.Count);
        Assert.Contains(typeof(TestEventHandler), handlers);
        Assert.Contains(typeof(AnotherTestEventHandler), handlers);
    }

    [Fact]
    public void GetEventHandlers_WithDifferentEventTypes_ShouldReturnOnlyMatchingHandlers()
    {
        // Arrange
        _registry.Register<TestEventHandler>();
        _registry.Register<AnotherTestEventEventHandler>();

        // Act
        var testEventHandlers = _registry.GetEventHandlersTypes(_testEvent);
        var anotherTestEventHandlers = _registry.GetEventHandlersTypes(_anotherTestEvent);

        // Assert
        Assert.Single(testEventHandlers);
        Assert.Contains(typeof(TestEventHandler), testEventHandlers);
        
        Assert.Single(anotherTestEventHandlers);
        Assert.Contains(typeof(AnotherTestEventEventHandler), anotherTestEventHandlers);
    }

    #endregion

    #region CreateHandlersForEvent Tests

    [Fact]
    public void CreateHandlersForEvent_WithNoRegisteredHandlers_ShouldReturnEmptyList()
    {
        // Act
        var handlers = _registry.GetEventHandlers(_testEvent);

        // Assert
        Assert.Empty(handlers);
    }

    [Fact]
    public void CreateHandlersForEvent_WithRegisteredHandlers_ShouldCreateHandlerInstances()
    {
        // Arrange
        _registry.Register<TestEventHandler>();
        _registry.Register<AnotherTestEventHandler>();

        // Act
        var handlers = _registry.GetEventHandlers(_testEvent);

        // Assert
        Assert.Equal(2, handlers.Count);
        Assert.All(handlers, handler => Assert.NotNull(handler));
        Assert.All(handlers, handler => Assert.IsAssignableFrom<IEventHandler<TestEvent>>(handler));
    }

    [Fact]
    public void CreateHandlersForEvent_WithNullEvent_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _registry.GetEventHandlers<TestEvent>(null));
    }

    [Fact]
    public void CreateHandlersForEvent_ShouldCacheHandlerInstances()
    {
        // Arrange
        _registry.Register<TestEventHandler>();

        // Act
        var handlers1 = _registry.GetEventHandlers(_testEvent);
        var handlers2 = _registry.GetEventHandlers(_testEvent);

        // Assert
        Assert.Single(handlers1);
        Assert.Single(handlers2);
        Assert.Same(handlers1[0], handlers2[0]);
    }
    #endregion

    #region FromNamespace Tests

    [Fact]
    public void FromNamespace_WithValidNamespace_ShouldRegisterHandlers()
    {
        // Act
        _registry.FromNamespace("Magnett.Automation.Core.UnitTest.Events.Fakes");

        // Assert
        var handlers = _registry.GetEventHandlersTypes(_testEvent);
        Assert.Contains(typeof(TestEventHandler), handlers);
        Assert.Contains(typeof(AnotherTestEventHandler), handlers);
    }

    [Fact]
    public void FromNamespace_WithNullNamespace_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _registry.FromNamespace(null));
    }

    [Fact]
    public void FromNamespace_WithEmptyNamespace_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _registry.FromNamespace(""));
    }

    [Fact]
    public void FromNamespace_WithWhitespaceNamespace_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _registry.FromNamespace("   "));
    }

    [Fact]
    public void FromNamespace_WithNonExistentNamespace_ShouldReturnEmptyList()
    {
        // Act
        _registry.FromNamespace("NonExistent.Namespace");

        // Assert
        var handlers = _registry.GetEventHandlersTypes(_testEvent);
        Assert.Empty(handlers);
    }

    #endregion

    #region FromAssembly Tests

    [Fact]
    public void FromAssembly_WithCurrentAssembly_ShouldRegisterHandlers()
    {
        // Arrange
        var currentAssembly = Assembly.GetExecutingAssembly();

        // Act
        _registry.FromAssembly(currentAssembly.GetName().Name);

        // Assert
        var handlers = _registry.GetEventHandlersTypes(_testEvent);
        Assert.Contains(typeof(TestEventHandler), handlers);
        Assert.Contains(typeof(AnotherTestEventHandler), handlers);
    }

    [Fact]
    public void FromAssembly_WithNullAssemblyName_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _registry.FromAssembly(null));
    }

    [Fact]
    public void FromAssembly_WithEmptyAssemblyName_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _registry.FromAssembly(""));
    }

    [Fact]
    public void FromAssembly_WithWhitespaceAssemblyName_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _registry.FromAssembly("   "));
    }

    [Fact]
    public void FromAssembly_WithNonExistentAssembly_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => _registry.FromAssembly("NonExistentAssembly"));
    }

    #endregion

    #region Clear Tests

    [Fact]
    public void Clear_WithRegisteredHandlers_ShouldRemoveAllHandlers()
    {
        // Arrange
        _registry.Register<TestEventHandler>();
        _registry.Register<AnotherTestEventHandler>();

        // Act
        _registry.Clear();

        // Assert
        var handlers = _registry.GetEventHandlersTypes(_testEvent);
        Assert.Empty(handlers);
    }

    [Fact]
    public void Clear_ShouldClearCache()
    {
        // Arrange
        _registry.Register<TestEventHandler>();
        var handlers1 = _registry.GetEventHandlers(_testEvent);

        // Act
        _registry.Clear();
        var handlers2 = _registry.GetEventHandlers(_testEvent);

        // Assert
        Assert.Empty(handlers2);
    }

    [Fact]
    public void Clear_ShouldReturnSelfForFluentApi()
    {
        // Act
        var result = _registry.Clear();

        // Assert
        Assert.Same(_registry, result);
    }

    #endregion

    #region Fluent API Tests

    [Fact]
    public void FluentApi_ShouldAllowChaining()
    {
        // Act
        var result = _registry
            .Register<TestEventHandler>()
            .Register<AnotherTestEventHandler>()
            .FromNamespace("Magnett.Automation.Core.UnitTest.Events.Fakes")
            .Clear()
            .Register<TestEventHandler>();

        // Assert
        Assert.Same(_registry, result);
        var handlers = _registry.GetEventHandlersTypes(_testEvent);
        Assert.Single(handlers);
        Assert.Contains(typeof(TestEventHandler), handlers);
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_ShouldDisposeCache()
    {
        // Arrange
        var registry = EventHandlerRegistry.Create(_mockLogger.Object);
        registry.Register<TestEventHandler>();

        // Act
        registry.Dispose();

        // Assert
        // Should not throw when accessing disposed cache
        var handlers = registry.GetEventHandlersTypes(_testEvent);
        Assert.Empty(handlers); // Cache should be cleared
    }

    [Fact]
    public void Dispose_ShouldNotThrowOnMultipleCalls()
    {
        // Arrange
        var registry = EventHandlerRegistry.Create(_mockLogger.Object);

        // Act & Assert
        var exception1 = Record.Exception(() => registry.Dispose());
        var exception2 = Record.Exception(() => registry.Dispose());

        Assert.Null(exception1);
        Assert.Null(exception2);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Integration_CompleteWorkflow_ShouldWorkCorrectly()
    {
        // Arrange
        var testEvent = new TestEvent("IntegrationTest", "IntegrationCaller");

        // Act
        _registry
            .Register<TestEventHandler>()
            .Register<AnotherTestEventHandler>();

        var handlerTypes = _registry.GetEventHandlersTypes(testEvent);
        var handlers = _registry.GetEventHandlers(testEvent);

        // Assert
        Assert.Equal(2, handlerTypes.Count);
        Assert.Equal(2, handlers.Count);

        // Verify handlers can actually handle events
        foreach (var handler in handlers)
        {
            var task = handler.Handle(testEvent, _mockLogger.Object);
            Assert.True(task.IsCompleted);
        }
    }

    [Fact]
    public void Integration_MultipleEventTypes_ShouldHandleCorrectly()
    {
        // Arrange
        _registry.Register<TestEventHandler>();
        _registry.Register<AnotherTestEventEventHandler>();

        // Act
        var testEventHandlers = _registry.GetEventHandlers(_testEvent);
        var anotherTestEventHandlers = _registry.GetEventHandlers(_anotherTestEvent);

        // Assert
        Assert.Single(testEventHandlers);
        Assert.IsType<TestEventHandler>(testEventHandlers[0]);

        Assert.Single(anotherTestEventHandlers);
        Assert.IsType<AnotherTestEventEventHandler>(anotherTestEventHandlers[0]);
    }

    #endregion
} 