# Magnett.Automation.Core

[![NuGet](https://img.shields.io/nuget/v/Magnett.Automation.Core)](https://www.nuget.org/packages/Magnett.Automation.Core)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Magnett.Automation.Core)](https://www.nuget.org/packages/Magnett.Automation.Core)
[![.NET](https://img.shields.io/badge/.NET-6.0%20%7C%207.0%20%7C%208.0%20%7C%209.0-blue)](https://dotnet.microsoft.com/download)

A powerful .NET library for building workflows, orchestrating processes, and managing state machines with event-driven architecture.

## 🚀 Quick Start

### Installation
```bash
dotnet add package Magnett.Automation.Core
```

### Basic State Machine Example
```csharp
using Magnett.Automation.Core;

// Define states and actions
public record OrderState : Enumeration
{
    public static readonly OrderState Pending = new("Pending");
    public static readonly OrderState Processing = new("Processing");
    public static readonly OrderState Completed = new("Completed");
    
    private OrderState(string name) : base(name) { }
}

public record OrderAction : Enumeration
{
    public static readonly OrderAction Process = new("Process");
    public static readonly OrderAction Complete = new("Complete");
    
    private OrderAction(string name) : base(name) { }
}

// Create state machine definition
var definition = MachineDefinitionBuilder.Create()
    .InitialState(OrderState.Pending)
        .OnAction(OrderAction.Process).ToState(OrderState.Processing)
        .Build()
    .AddState(OrderState.Processing)
        .OnAction(OrderAction.Complete).ToState(OrderState.Completed)
        .Build()
    .BuildDefinition();

// Use the state machine
var eventBus = EventBus.Create(logger);
var machine = await Machine.CreateAsync(definition, eventBus);
await machine.DispatchAsync(OrderAction.Process);
```

### Basic Workflow Example
```csharp
// Define workflow nodes and transitions
var definition = FlowDefinitionBuilder.Create()
    .WithInitialNode<ProcessOrderNode>(NodeName.Process)
        .OnExitCode(ExitCode.Success).GoTo(NodeName.Notify)
        .OnExitCode(ExitCode.Failed).GoTo(NodeName.HandleError)
        .Build()
    .WithNode<NotifyCustomerNode>(NodeName.Notify)
        .Build()
    .WithNode<HandleErrorNode>(NodeName.HandleError)
        .Build()
    .BuildDefinition();

// Execute workflow
var context = Context.Create();
var flow = Flow.Create(FlowRunner.Create(definition, context));
var result = await flow.Run();
```

## ✨ Key Features

- **🔄 Workflows**: Declarative workflow orchestration with async/sync nodes
- **📊 State Machines**: Type-safe state management with transitions
- **📡 Events**: Event-driven architecture with EventBus and EventStream
- **📈 Metrics**: Built-in performance monitoring and observability
- **💾 Context**: Shared data storage across workflow components
- **🎯 Type-Safe**: Full IntelliSense support with generic types

## 🏗️ Core Components

### Event-Driven Architecture
```csharp
// Simple setup with automatic metrics
var eventBus = EventBus.Create(logger);

// Publish events
await eventBus.PublishAsync(new OrderCreatedEvent(orderId));

// Register handlers
eventBus.EventHandlerRegistry.Register<OrderEventHandler>();
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

## 📊 Built-in Observability

The library includes comprehensive metrics and event monitoring:

```csharp
// Access metrics through the event bus
var metricsRegistry = GetMetricsRegistryFromEventBus(eventBus);
var processingTime = metricsRegistry.GetHistogramValues("events.processing.time");
var eventCount = metricsRegistry.GetCounterValue("events.published");
```

### Automatic Event Monitoring
- **Node Lifecycle Events**: `OnNodeInitEvent`, `OnNodeExecuteEvent`, `OnNodeCompletedEvent`
- **State Machine Events**: `OnMachineInit`, `OnMachineChangeState`
- **Context Events**: `OnChangeFieldValueEvent`

## 🎯 Use Cases

- **Business Process Automation**: Automate complex business workflows
- **Microservices Orchestration**: Coordinate distributed services
- **Event-Driven Applications**: Build reactive, event-sourced systems
- **Workflow Engines**: Create custom workflow solutions
- **State Management**: Manage complex application states
- **Integration Patterns**: Implement saga patterns and distributed transactions

## 📦 Supported Frameworks

- .NET 6.0+
- .NET 7.0+
- .NET 8.0+
- .NET 9.0+

## 🔧 Requirements

- Microsoft.Extensions.Logging
- Microsoft.Extensions.Caching.Memory

## 📚 Documentation & Examples

- **[Complete Documentation](https://github.com/LHPiney/magnett-automation-core)** - Full API reference and guides
- **[Integration Tests](https://github.com/LHPiney/magnett-automation-core/tree/main/test/Magnett.Automation.Core.IntegrationTest)** - Complete working examples
- **[State Machine Examples](https://github.com/LHPiney/magnett-automation-core/tree/main/test/Magnett.Automation.Core.IntegrationTest/StateMachines)** - DocumentMachine, OrderMachine
- **[Saga Pattern Example](https://github.com/LHPiney/magnett-automation-core/tree/main/test/Magnett.Automation.Core.IntegrationTest/WorkFlows/SagaPattern)** - Distributed transaction management
- **[Event-Driven Examples](https://github.com/LHPiney/magnett-automation-core/tree/main/test/Magnett.Automation.Core.IntegrationTest/Events)** - EventBus usage patterns

## 🚀 Why Choose Magnett Automation Core?

- **🎯 Production Ready**: Battle-tested in real-world applications
- **🔒 Type Safe**: Compile-time safety with full IntelliSense support
- **📦 Lightweight**: Minimal dependencies, maximum functionality
- **🔄 Extensible**: Plugin architecture for custom implementations
- **📊 Observable**: Built-in metrics and event-driven monitoring
- **🚀 Fast**: Optimized for performance and scalability

## 🤝 Contributing

Contributions welcome! See our [Contributing Guide](https://github.com/LHPiney/magnett-automation-core/blob/main/CONTRIBUTING.md).

- **🐛 Report Issues**: [Bug Report Template](https://github.com/LHPiney/magnett-automation-core/issues/new?template=bug_report.yml)
- **✨ Suggest Features**: [Feature Request Template](https://github.com/LHPiney/magnett-automation-core/issues/new?template=feature_request.yml)
- **❓ Ask Questions**: [Support Template](https://github.com/LHPiney/magnett-automation-core/issues/new?template=support.yml)

## 📄 License

MIT License - see [LICENSE](https://github.com/LHPiney/magnett-automation-core/blob/main/LICENSE.md) file for details.

---

**Ready to build amazing workflows?** [Get started now!](#-quick-start)

[![GitHub stars](https://img.shields.io/github/stars/LHPiney/magnett-automation-core?style=social)](https://github.com/LHPiney/magnett-automation-core)
