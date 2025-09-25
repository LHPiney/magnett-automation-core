---
layout: default
title: Home
nav_order: 1
permalink: /
---

# Magnett Automation Core

A powerful .NET library for building event-driven, stateful automation workflows with comprehensive event handling, state machine management, and workflow orchestration capabilities.

## Features

- **Event System**: Asynchronous event publishing, handling, and streaming
- **State Machines**: Robust state management with transitions and actions
- **Workflow Orchestration**: Complex workflow definitions with nodes and links
- **Context Management**: Type-safe context handling with event emission
- **Metrics Integration**: Built-in performance monitoring and metrics collection
- **Cancellation Support**: Comprehensive cancellation token support throughout

## Quick Start

### Installation

```bash
dotnet add package Magnett.Automation.Core
```

### Basic Event Handling

```csharp
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;

// Create event bus
var logger = LoggerFactory.Create(builder => builder.AddConsole())
    .CreateLogger<EventBus>();
var eventBus = EventBus.Create(logger);

// Register event handler
eventBus.EventHandlerRegistry.Register<MyEventHandler>();

// Start processing
eventBus.Start();

// Publish event
await eventBus.PublishAsync(new MyEvent("Hello World"));
```

### Simple State Machine

```csharp
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Builders;

// Define state machine
var machineDefinition = MachineDefinitionBuilder.Create()
    .InitialState("Idle")
        .OnAction("Start").ToState("Running")
        .Build()
    .AddState("Running")
        .OnAction("Stop").ToState("Idle")
        .Build()
    .BuildDefinition();

// Create and use machine
var machine = await Machine.CreateAsync(machineDefinition, eventBus);
await machine.DispatchAsync("Start");
```

### Workflow Definition

```csharp
using Magnett.Automation.Core.WorkFlows.Definitions.Builders;

// Define workflow
var flowDefinition = FlowDefinitionBuilder.Create()
    .WithInitialNode<StartNode>(NodeName.Start)
        .OnExitCode("Success").GoTo(NodeName.Process)
        .Build()
    .WithNode<ProcessNode>(NodeName.Process)
        .OnExitCode("Complete").GoTo(NodeName.End)
        .Build()
    .WithNode<EndNode>(NodeName.End)
        .Build()
    .BuildDefinition();
```

## Architecture Overview

Magnett Automation Core is built around four main components:

1. **Event System**: Handles asynchronous event processing and distribution
2. **State Machines**: Manages application state transitions and business logic
3. **Workflows**: Orchestrates complex business processes with nodes and transitions
4. **Context Management**: Provides type-safe data storage and retrieval

## Examples

- [Saga Pattern Implementation](examples/saga-pattern/)
- [Event-Driven Architecture](examples/event-driven/)
- [State Machine Patterns](examples/state-machines/)

## Documentation

- [Getting Started](getting-started/)
- [Architecture Guide](architecture/)
- [API Reference](api/)
- [Patterns and Best Practices](patterns/)

## Contributing

We welcome contributions! Please see our [Contributing Guide](contributing/) for details.

## License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE.md) file for details.
