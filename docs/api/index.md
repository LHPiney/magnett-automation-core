---
layout: default
title: API Reference
nav_order: 5
permalink: /api/
---

# API Reference

This page provides complete documentation of the Magnett Automation Core API.

## Event Bus

### EventBus

The EventBus is the core of the event system.

```csharp
public class EventBus : IEventBus
{
    public static EventBus Create(ILogger<EventBus> logger);
    public void Start();
    public Task StopAsync();
    public Task PublishAsync<T>(T @event) where T : IEvent;
    public IEventHandlerRegistry EventHandlerRegistry { get; }
}
```

#### Methods

- **Create(ILogger<EventBus> logger)**: Creates a new EventBus instance
- **Start()**: Starts the EventBus
- **StopAsync()**: Stops the EventBus asynchronously
- **PublishAsync<T>(T @event)**: Publishes an event asynchronously
- **EventHandlerRegistry**: Gets the event handler registry

### IEventHandlerRegistry

Event handler registry.

```csharp
public interface IEventHandlerRegistry
{
    void Register<T>() where T : class, IEventHandler;
    void Register<T>(T handler) where T : class, IEventHandler;
    void RegisterFromNamespace(string @namespace);
    void RegisterFromAssembly(Assembly assembly);
    IEnumerable<IEventHandler> GetHandlers<T>() where T : IEvent;
}
```

#### Methods

- **Register<T>()**: Registers an event handler by type
- **Register<T>(T handler)**: Registers a handler instance
- **RegisterFromNamespace(string @namespace)**: Registers all handlers from a namespace
- **RegisterFromAssembly(Assembly assembly)**: Registers all handlers from an assembly
- **GetHandlers<T>()**: Gets all handlers for an event type

## State Machines

### MachineDefinitionBuilder

Builder for defining state machines.

```csharp
public class MachineDefinitionBuilder
{
    public static MachineDefinitionBuilder Create();
    public MachineDefinitionBuilder WithInitialState(string stateName);
    public StateBuilder WithState(string stateName);
    public IMachineDefinition Build();
}
```

#### Methods

- **Create()**: Creates a new builder instance
- **WithInitialState(string stateName)**: Defines the initial state
- **WithState(string stateName)**: Defines a state
- **Build()**: Builds the machine definition

### StateBuilder

Builder for defining states.

```csharp
public class StateBuilder
{
    public StateBuilder OnAction(string actionName);
    public StateBuilder GoTo(string targetState);
    public StateBuilder Build();
}
```

#### Methods

- **OnAction(string actionName)**: Defines an action for the state
- **GoTo(string targetState)**: Defines the target state
- **Build()**: Builds the state

### Machine

State machine implementation.

```csharp
public class Machine : IMachine
{
    public static Machine Create(IMachineDefinition definition, IEventBus eventBus);
    public Task DispatchAsync(string action);
    public string CurrentState { get; }
    public IMachineDefinition Definition { get; }
}
```

#### Methods

- **Create(IMachineDefinition definition, IEventBus eventBus)**: Creates a new instance
- **DispatchAsync(string action)**: Executes an action asynchronously
- **CurrentState**: Gets the current state
- **Definition**: Gets the machine definition

## Workflows

### NodeAsync

Base class for asynchronous workflow nodes.

```csharp
public abstract class NodeAsync : INode
{
    protected NodeAsync(CommonNamedKey name, IEventBus eventBus);
    protected abstract Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default);
    public CommonNamedKey Name { get; }
    public IEventBus EventBus { get; }
}
```

#### Properties

- **Name**: Node name
- **EventBus**: Associated event bus

#### Methods

- **HandleAsync(Context context, CancellationToken cancellationToken)**: Handles node logic

### NodeExit

Workflow node execution result.

```csharp
public record NodeExit
{
    public static NodeExit Completed(ExitCode code, string message = null);
    public static NodeExit Failed(ExitCode code, string message = null);
    public static NodeExit Paused(ExitCode code, string message = null);
    public static NodeExit Cancelled(ExitCode code, string message = null);
    
    public ExitCode Code { get; }
    public ExitState State { get; }
    public string Message { get; }
    public object Data { get; }
}
```

#### Static Methods

- **Completed(ExitCode code, string message)**: Creates a completed result
- **Failed(ExitCode code, string message)**: Creates a failed result
- **Paused(ExitCode code, string message)**: Creates a paused result
- **Cancelled(ExitCode code, string message)**: Creates a cancelled result

### ExitCode

Exit codes for workflow nodes.

```csharp
public record ExitCode : Enumeration
{
    public static readonly ExitCode Done = new(1, nameof(Done));
    public static readonly ExitCode Created = new(2, nameof(Created));
    public static readonly ExitCode PreAuthorized = new(3, nameof(PreAuthorized));
    public static readonly ExitCode Denied = new(4, nameof(Denied));
    public static readonly ExitCode Cancelled = new(5, nameof(Cancelled));
    public static readonly ExitCode Failed = new(6, nameof(Failed));
}
```

### ExitState

Exit states for workflow nodes.

```csharp
public record ExitState : Enumeration
{
    public static readonly ExitState Completed = new(1, nameof(Completed));
    public static readonly ExitState Failed = new(2, nameof(Failed));
    public static readonly ExitState Paused = new(3, nameof(Paused));
    public static readonly ExitState Cancelled = new(4, nameof(Cancelled));
}
```

## Context Management

### Context

Shared context for workflows.

```csharp
public class Context : IContext
{
    public T Value<T>(ContextField<T> field);
    public Task StoreAsync<T>(ContextField<T> field, T value);
    public bool HasValue<T>(ContextField<T> field);
    public void Remove<T>(ContextField<T> field);
}
```

#### Methods

- **Value<T>(ContextField<T> field)**: Gets a value from context
- **StoreAsync<T>(ContextField<T> field, T value)**: Stores a value in context
- **HasValue<T>(ContextField<T> field)**: Checks if a value exists
- **Remove<T>(ContextField<T> field)**: Removes a value from context

### ContextField

Typed context field.

```csharp
public class ContextField<T> : CommonNamedKey
{
    public static ContextField<T> WithName(string name);
    public static ContextField<T> WithKey(CommonNamedKey key);
}
```

#### Static Methods

- **WithName(string name)**: Creates a field with name
- **WithKey(CommonNamedKey key)**: Creates a field with key

## Events

### Event

Base class for events.

```csharp
public abstract record Event : IEvent
{
    protected Event(string name, string source);
    public string Name { get; }
    public string Source { get; }
    public DateTime Timestamp { get; }
    public string Id { get; }
}
```

#### Properties

- **Name**: Event name
- **Source**: Event source
- **Timestamp**: Timestamp
- **Id**: Unique identifier

### IEventHandler

Interface for event handlers.

```csharp
public interface IEventHandler<T> where T : IEvent
{
    Task Handle(T @event, ILogger logger, CancellationToken cancellationToken = default);
}
```

#### Methods

- **Handle(T @event, ILogger logger, CancellationToken cancellationToken)**: Handles an event

## Metrics

### IMetricsCollector

Interface for metrics collection.

```csharp
public interface IMetricsCollector
{
    void IncrementCounter(string name, int value = 1);
    void RecordDuration(string name, TimeSpan duration);
    void RecordGauge(string name, double value);
    void RecordHistogram(string name, double value);
}
```

#### Methods

- **IncrementCounter(string name, int value)**: Increments a counter
- **RecordDuration(string name, TimeSpan duration)**: Records a duration
- **RecordGauge(string name, double value)**: Records a gauge
- **RecordHistogram(string name, double value)**: Records a histogram

### IMetricsRegistry

Metrics registry.

```csharp
public interface IMetricsRegistry
{
    void RegisterCounter(string name);
    void RegisterDuration(string name);
    void RegisterGauge(string name);
    void RegisterHistogram(string name);
    IMetricsViewer GetViewer();
}
```

#### Methods

- **RegisterCounter(string name)**: Registers a counter
- **RegisterDuration(string name)**: Registers a duration
- **RegisterGauge(string name)**: Registers a gauge
- **RegisterHistogram(string name)**: Registers a histogram
- **GetViewer()**: Gets a metrics viewer

## Commons

### Enumeration

Base class for enumerations.

```csharp
public abstract record Enumeration
{
    protected Enumeration(int id, string name);
    public int Id { get; }
    public string Name { get; }
    public override string ToString();
    public override bool Equals(object obj);
    public override int GetHashCode();
}
```

#### Properties

- **Id**: Numeric identifier
- **Name**: Enumeration name

### CommonNamedKey

Common named key.

```csharp
public record CommonNamedKey
{
    public static CommonNamedKey WithName(string name);
    public static CommonNamedKey WithKey(string key);
    public string Name { get; }
    public string Key { get; }
}
```

#### Static Methods

- **WithName(string name)**: Creates a key with name
- **WithKey(string key)**: Creates a key with key

## Exceptions

### StateMachineException

Exception for state machine errors.

```csharp
public class StateMachineException : Exception
{
    public StateMachineException(string message);
    public StateMachineException(string message, Exception innerException);
}
```

### WorkflowException

Exception for workflow errors.

```csharp
public class WorkflowException : Exception
{
    public WorkflowException(string message);
    public WorkflowException(string message, Exception innerException);
}
```

### EventBusException

Exception for event bus errors.

```csharp
public class EventBusException : Exception
{
    public EventBusException(string message);
    public EventBusException(string message, Exception innerException);
}
```

## Usage Examples

### Create Event Bus

```csharp
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
eventBus.EventHandlerRegistry.Register<MyEventHandler>();
eventBus.Start();
```

### Create State Machine

```csharp
var definition = MachineDefinitionBuilder.Create()
    .WithInitialState("Initial")
    .WithState("Initial")
        .OnAction("Next").GoTo("Next")
        .Build()
    .Build();

var machine = Machine.Create(definition, eventBus);
await machine.DispatchAsync("Next");
```

### Create Workflow Node

```csharp
public class MyNode : NodeAsync
{
    private readonly ContextField<string> _fieldField = ContextField<string>.WithName("Field");
    
    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        var value = context.Value(_fieldField);
        return NodeExit.Completed(ExitCode.Done, $"Processed: {value}");
    }
}
```

### Create Event

```csharp
public record MyEvent(string Data) : Event("MyEvent", "MyService");

public class MyEventHandler : IEventHandler<MyEvent>
{
    public Task Handle(MyEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        logger.LogInformation("Event received: {@event}", @event);
        return Task.CompletedTask;
    }
}
```
