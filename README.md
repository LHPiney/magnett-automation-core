# Magnett Automation Core

[![Build](https://github.com/LHPiney/magnett-automation-core/actions/workflows/build-and-analyze.yml/badge.svg)](https://github.com/LHPiney/magnett-automation-core/actions/workflows/build-and-analyze.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=magnett_automation&metric=alert_status)](https://sonarcloud.io/dashboard?id=magnett_automation)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=magnett_automation&metric=coverage)](https://sonarcloud.io/dashboard?id=magnett_automation)
![NuGet](https://img.shields.io/nuget/v/Magnett.Automation.Core)
![NuGet Downloads](https://img.shields.io/nuget/dt/Magnett.Automation.Core)
![.NET](https://img.shields.io/badge/.NET-6.0%20%7C%207.0%20%7C%208.0%20%7C%209.0-blue)

<div align="center">
  <img src="./assets/logo.png" alt="Magnett Automation" width="300">
</div>

**A powerful .NET library for building workflows, orchestrating processes, and managing state machines with event-driven architecture.**

## 📖 What is Magnett Automation Core?

Magnett Automation Core is a comprehensive .NET library designed to simplify the creation of complex, event-driven applications. It provides a unified approach to workflow orchestration, state management, and process automation that scales from simple business processes to complex microservices architectures.

### 🎯 What it Solves

**Traditional Challenges:**
- Complex nested if/else statements for business logic
- Difficult-to-maintain state management
- Scattered event handling across applications
- Manual orchestration of distributed processes
- Lack of observability in automated workflows

**Our Solution:**
- **Declarative Workflows**: Define business processes as clear, maintainable workflows
- **Event-Driven Architecture**: Built-in event system for loose coupling and scalability
- **State Machine Management**: Type-safe state transitions with clear business rules
- **Process Orchestration**: Coordinate complex multi-step processes automatically
- **Built-in Observability**: Metrics, logging, and monitoring out of the box

### 🏗️ Core Philosophy

**Separation of Concerns**: Clear separation between workflow definition and execution
**Type Safety**: Full IntelliSense support with compile-time validation
**Event-Driven**: Reactive architecture that responds to changes and events
**Observable**: Built-in metrics and monitoring for production environments
**Extensible**: Plugin architecture for custom implementations

### 🎯 Target Use Cases

- **Business Process Automation**: Automate complex business workflows
- **Microservices Orchestration**: Coordinate distributed services
- **Event-Driven Applications**: Build reactive, event-sourced systems
- **Workflow Engines**: Create custom workflow solutions
- **State Management**: Manage complex application states
- **Integration Patterns**: Implement saga patterns and distributed transactions

## 🚀 Quick Start

### Installation
```bash
dotnet add package Magnett.Automation.Core
```

### Basic Workflow Example
```csharp
using Magnett.Automation.Core;

// Create context and event bus with automatic metrics
var context = Context.Create();
var eventBus = EventBus.CreateWithDefaultMetrics(logger);

// Define workflow with nodes and transitions
var definition = FlowDefinitionBuilder.Create()
    .WithInitialNode<ResetValueNode>(NodeName.Reset)
        .OnExitCode(ExitCode.Ok).GoTo(NodeName.SetValue)
        .Build()
    .WithNode<SetValueNode>(NodeName.SetValue)
        .OnExitCode(ExitCode.Assigned).GoTo(NodeName.SumValue)
        .Build()
    .WithNode<SumValueNode>(NodeName.SumValue)
        .Build()
    .BuildDefinition();

// Execute workflow
var flow = Flow.Create(FlowRunner.Create(definition, context));
var result = await flow.Run();
```

### Complete Examples
For comprehensive examples including:
- **Simple Workflow**: Basic 3-node workflow with context management
- **Saga Pattern**: Distributed transaction management
- **State Machines**: Complex state transitions
- **Event-Driven Architecture**: EventBus integration

See the [Integration Tests](test/Magnett.Automation.Core.IntegrationTest/) directory for complete, working examples.

## ✨ Key Features

- **🔄 Workflows**: Declarative workflow orchestration with async/sync nodes
- **📊 State Machines**: Type-safe state management with transitions
- **📡 Events**: Event-driven architecture with EventBus and EventStream
- **📈 Metrics**: Built-in performance monitoring and observability
- **💾 Context**: Shared data storage across workflow components
- **🎯 Type-Safe**: Full IntelliSense support with generic types

## 🏗️ Architecture Overview

Magnett Automation Core is built around five core pillars:

### 🔄 **Workflows**
Declarative workflow definitions that separate business logic from execution. Define complex processes as a series of connected nodes with clear transitions and error handling.

### 📊 **State Machines**
Type-safe state management with explicit transitions. Define business states and the actions that can transition between them, with compile-time validation.

### 📡 **Events**
Event-driven architecture with asynchronous processing. Publish events, register handlers, and build reactive systems that respond to changes in real-time.

### 💾 **Context**
Shared data storage that flows through workflows and state machines. Type-safe field definitions with automatic serialization and event emission on changes.

### 🔗 **Integration**
All components work together seamlessly:
- **Workflows** can emit events and manage state
- **State Machines** can trigger workflows and emit events
- **Events** can drive workflow transitions and state changes
- **Context** provides shared data across all components

## 📦 Core Components

### Events System
```csharp
// Simple setup with automatic metrics
var eventBus = EventBus.CreateWithDefaultMetrics(logger);

// Publish events
await eventBus.PublishAsync(new OrderCreatedEvent(orderId));

// Register handlers
eventBus.EventHandlerRegistry.Register<OrderEventHandler>();
```

### State Machine
```csharp
// Define states and actions
public class OrderState : Enumeration
{
    public static readonly OrderState Pending = new(1, nameof(Pending));
    public static readonly OrderState Processing = new(2, nameof(Processing));
    public static readonly OrderState Completed = new(3, nameof(Completed));
}

// Create machine definition
var definition = MachineDefinitionBuilder.Create()
    .InitialState(OrderState.Pending)
        .OnAction(OrderAction.Process).ToState(OrderState.Processing)
        .Build()
    .AddState(OrderState.Processing)
        .OnAction(OrderAction.Complete).ToState(OrderState.Completed)
        .Build()
    .BuildDefinition();

// Use the machine
var machine = Machine.Create(definition);
machine.Dispatch(OrderAction.Process);
```

### Context Management
```csharp
// Create context
var context = Context.Create();

// Define typed fields
var orderIdField = ContextField<int>.Create("OrderId");
var statusField = ContextField<string>.Create("Status");

// Store and retrieve values
context.Store(orderIdField, 12345);
context.Store(statusField, "Processing");

var orderId = context.Value(orderIdField);
var status = context.Value(statusField);
```

### Workflow Nodes
```csharp
// Define node names
public record NodeName : CommonNamedKey 
{
    public static readonly NodeName Reset = new("Reset");
    public static readonly NodeName Process = new("Process");
    
    private NodeName(string name) : base(name) { }
}

// Define exit codes
public record ExitCode : Enumeration
{
    public static readonly ExitCode Success = new(1, nameof(Success));
    public static readonly ExitCode Failed = new(2, nameof(Failed));
    
    private ExitCode(int id, string name) : base(id, name) { }
}

// Implement workflow node
public class ProcessOrderNode : Node
{
    public ProcessOrderNode(CommonNamedKey key, IEventBus eventBus) : base(key, eventBus) { }

    protected override NodeExit Handle(Context context)
    {
        // Your business logic here
        var orderId = context.Value(ContextDefinition.OrderId);
        
        // Process order...
        
        return NodeExit.Completed(ExitCode.Success);
    }
}
```

## 📊 Metrics & Observability

Built-in metrics collection for monitoring workflow performance:

```csharp
// Simple setup with automatic metrics
var eventBus = EventBus.Create(logger);

// Access metrics through the event bus
var metricsRegistry = GetMetricsRegistryFromEventBus(eventBus);
var processingTime = metricsRegistry.GetHistogramValues("events.processing.time");
var eventCount = metricsRegistry.GetCounterValue("events.published");
```

**Advanced Configuration:**
```csharp
// Enable metrics manually for advanced scenarios
var metricsRegistry = new MetricsRegistry();
var metricsCollector = new MetricsCollector(metricsRegistry);
var eventBus = EventBus.Create(logger, null, metricsCollector);

// Access metrics
var processingTime = metricsRegistry.GetHistogramValues("events.processing.time");
var eventCount = metricsRegistry.GetCounterValue("events.published");
```

## 📡 Workflow Events

During workflow execution, the system automatically emits different types of events that you can monitor and handle:

### Node Lifecycle Events

| Event | Description | Properties |
|-------|-------------|------------|
| `OnNodeInitEvent` | Emitted when a node is initialized | `NodeName` |
| `OnNodeExecuteEvent` | Emitted when a node is executed | `NodeName` |
| `OnNodeCompletedEvent` | Emitted when a node completes successfully | `NodeName`, `Code`, `Data` |
| `OnNodeFailedEvent` | Emitted when a node fails | `NodeName`, `Code`, `Data` |
| `OnNodeCancelledEvent` | Emitted when a node is cancelled | `NodeName`, `Code`, `Data` |
| `OnNodePausedEvent` | Emitted when a node is paused | `NodeName`, `Code`, `Data` |

### State Machine Events

| Event | Description | Properties |
|-------|-------------|------------|
| `OnMachineInit` | Emitted when a state machine is initialized | `MachineId` |
| `OnMachineChangeState` | Emitted when a state machine changes state | `MachineId`, `SourceState`, `TargetState`, `Action` |

### Context Events

| Event | Description | Properties |
|-------|-------------|------------|
| `OnChangeFieldValueEvent` | Emitted when a context field value changes | `FieldName`, `ValueType`, `Value`, `PreviousValue` |

### Real-time Event Monitoring

```csharp
// Simple setup with automatic metrics collection
var eventBus = EventBus.CreateWithDefaultMetrics(logger);

// Register handlers for specific events
eventBus.EventHandlerRegistry.Register<OnNodeExecuteEventHandler>();
eventBus.EventHandlerRegistry.Register<OnMachineChangeStateHandler>();

// Access metrics through the event bus
var metricsRegistry = GetMetricsRegistryFromEventBus(eventBus);
var publishedCount = metricsRegistry.GetCounterValue("events.published");
var processedCount = metricsRegistry.GetCounterValue("events.processed");
var queueSize = metricsRegistry.GetGaugeValue("queue.size");
```

**Alternative: Manual Configuration**
```csharp
// For advanced scenarios, configure metrics manually
var metricsRegistry = new MetricsRegistry();
var metricsCollector = new MetricsCollector(metricsRegistry);
var eventBus = EventBus.Create(logger, null, metricsCollector);

// Monitor metrics in real-time
var publishedCount = metricsRegistry.GetCounterValue("events.published");
var processedCount = metricsRegistry.GetCounterValue("events.processed");
var queueSize = metricsRegistry.GetGaugeValue("queue.size");
```

## 🎯 Why Choose Magnett Automation Core?

### 🚀 **Productivity Boost**
- **Declarative Approach**: Define complex processes in clear, maintainable code
- **Type Safety**: Catch errors at compile time, not runtime
- **IntelliSense Support**: Full IDE support with autocomplete and documentation
- **Rapid Development**: Build complex workflows in minutes, not hours

### 🏗️ **Enterprise Ready**
- **Production Tested**: Battle-tested in real-world applications
- **High Performance**: Optimized for throughput and low latency
- **Observable**: Built-in metrics, logging, and monitoring
- **Scalable**: Designed for high-concurrency scenarios

### 🔧 **Developer Experience**
- **Clean API**: Intuitive, fluent interfaces
- **Comprehensive Examples**: Complete working examples in integration tests
- **Active Community**: Responsive maintainers and growing community
- **Well Documented**: Extensive documentation and examples

## 🎯 Use Cases

- **Microservices Orchestration**: Coordinate complex service interactions
- **Business Process Automation**: Automate multi-step business workflows
- **Event-Driven Applications**: Build reactive systems with event sourcing
- **State Management**: Manage complex application states
- **Workflow Engines**: Create custom workflow solutions
- **Integration Patterns**: Implement saga patterns and distributed transactions

## 🔧 Requirements

- .NET 6.0 or later
- Microsoft.Extensions.Logging
- Microsoft.Extensions.Caching.Memory

## 📚 Documentation

- [Integration Tests](test/Magnett.Automation.Core.IntegrationTest/) - Complete working examples
- [Unit Tests](test/Magnett.Automation.Core.UnitTest/) - API usage examples
- [Release Notes](RELEASE-NOTES.MD)

## 🤝 Contributing

We welcome contributions! Here's how you can help:

- **🐛 Report Issues**: Found a bug? [Use our bug report template](https://github.com/LHPiney/magnett-automation-core/issues/new?template=bug_report.yml)
- **✨ Suggest Features**: Have an idea? [Use our feature request template](https://github.com/LHPiney/magnett-automation-core/issues/new?template=feature_request.yml)
- **❓ Ask Questions**: Need help? [Use our support template](https://github.com/LHPiney/magnett-automation-core/issues/new?template=support.yml)
- **💬 Share Feedback**: Want to discuss? [Use our feedback template](https://github.com/LHPiney/magnett-automation-core/issues/new?template=feedback.yml)
- **🔧 Submit PRs**: Want to contribute code? [Check our contributing guide](CONTRIBUTING.md)
- **⭐ Star the Project**: Show your support by starring this repository

### Development Setup
```bash
git clone https://github.com/LHPiney/magnett-automation-core.git
cd magnett-automation-core
dotnet restore
dotnet build
dotnet test

# Run integration tests for complete examples
dotnet test test/Magnett.Automation.Core.IntegrationTest/
```

## 📈 Performance

- **High Performance**: Optimized for throughput and low latency
- **Memory Efficient**: Minimal allocations and garbage collection pressure
- **Scalable**: Designed for high-concurrency scenarios
- **Observable**: Built-in metrics and monitoring capabilities

## 🏆 Why Choose Magnett Automation Core?

- **🎯 Production Ready**: Battle-tested in real-world applications
- **🔒 Type Safe**: Compile-time safety with full IntelliSense support
- **📦 Lightweight**: Minimal dependencies, maximum functionality
- **🔄 Extensible**: Plugin architecture for custom implementations
- **📊 Observable**: Built-in metrics and event-driven monitoring
- **🚀 Fast**: Optimized for performance and scalability

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.md) file for details.

## 🙏 Acknowledgments

- Built with ❤️ by the Magnett team
- Inspired by modern workflow orchestration patterns
- Community-driven development and feedback

---

**Ready to build amazing workflows?** [Get started now!](#-quick-start)

[![GitHub stars](https://img.shields.io/github/stars/LHPiney/magnett-automation-core?style=social)](https://github.com/LHPiney/magnett-automation-core)
[![Twitter Follow](https://img.shields.io/twitter/follow/LHPiney?style=social)](https://twitter.com/LHPiney)