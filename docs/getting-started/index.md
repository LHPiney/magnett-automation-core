---
layout: default
title: Getting Started
nav_order: 2
has_children: true
permalink: /getting-started/
---

# Getting Started

This guide will help you get started with Magnett Automation Core quickly.

## Installation

### Prerequisites

- .NET 6.0 or higher
- Jetbrains Rider, Visual Studio 2022, VS Code
- Basic knowledge of C# and asynchronous programming

### Install Package

```bash
dotnet add package Magnett.Automation.Core
```

Or using Package Manager Console in Visual Studio:

```powershell
Install-Package Magnett.Automation.Core
```

## First Steps

### 1. Create a Project

```bash
dotnet new console -n MyAutomationProject
cd MyAutomationProject
dotnet add package Magnett.Automation.Core
```

### 2. Configure Event Bus

```csharp
using Magnett.Automation.Core.Events.Implementations;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
eventBus.Start();
```

### 3. Create Your First State Machine

```csharp
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Builders;

var definition = MachineDefinitionBuilder.Create()
    .WithInitialState("Inactive")
    .WithState("Inactive")
        .OnAction("Activate").GoTo("Active")
        .Build()
    .WithState("Active")
        .OnAction("Deactivate").GoTo("Inactive")
        .Build()
    .Build();

var machine = Machine.Create(definition, eventBus);
await machine.DispatchAsync("Activate");
```

### 4. Create Your First Workflow

```csharp
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;
using Magnett.Automation.Core.WorkFlows.Runtimes;

public class ProcessOrderNode : NodeAsync
{
    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        var orderId = context.Value<string>("OrderId");
        
        // Processing logic here
        Console.WriteLine($"Processing order: {orderId}");
        
        return NodeExit.Completed(ExitCode.Done, $"Order {orderId} processed successfully");
    }
}
```

## Basic Concepts

### States and Transitions

States represent different phases of a process, and transitions define how to move between them.

```csharp
var definition = MachineDefinitionBuilder.Create()
    .WithInitialState("Initial")
    .WithState("Initial")
        .OnAction("Next").GoTo("Intermediate")
        .Build()
    .WithState("Intermediate")
        .OnAction("Finish").GoTo("Final")
        .OnAction("Back").GoTo("Initial")
        .Build()
    .WithState("Final")
        .Build()
    .Build();
```

### Events

Events enable asynchronous communication between components.

```csharp
public record OrderCreatedEvent(string OrderId, decimal Amount) : Event("OrderCreated", "OrderService");

public class OrderCreatedHandler : IEventHandler<OrderCreatedEvent>
{
    public Task Handle(OrderCreatedEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Order created: {@event.OrderId}, Amount: {@event.Amount}");
        return Task.CompletedTask;
    }
}
```

### Shared Context

Context allows sharing data between workflow nodes.

```csharp
public class MyNode : NodeAsync
{
    private readonly ContextField<string> _nameField = ContextField<string>.WithName("Name");
    private readonly ContextField<int> _ageField = ContextField<int>.WithName("Age");
    
    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        var name = context.Value(_nameField);
        var age = context.Value(_ageField);
        
        // Use context data
        Console.WriteLine($"Name: {name}, Age: {age}");
        
        return NodeExit.Completed(ExitCode.Done, "Processed");
    }
}
```

## Next Steps

### Learn More

- [Architecture](architecture/) - Understand the system architecture
- [Patterns](patterns/) - Learn design patterns
- [Examples](examples/) - See practical examples
- [API Reference](api/) - Complete API documentation

### Additional Resources

- [GitHub Repository](https://github.com/magnett-automation-core/magnett-automation-core)
- [NuGet Package](https://www.nuget.org/packages/Magnett.Automation.Core)
- [Issues and Support](https://github.com/magnett-automation-core/magnett-automation-core/issues)

## Common Issues

### Compilation Error

If you encounter compilation errors, make sure you have:

1. .NET 6.0 or higher installed
2. Restored NuGet packages: `dotnet restore`
3. Cleaned and rebuilt: `dotnet clean && dotnet build`

### Performance Issues

To optimize performance:

1. Use asynchronous operations when possible
2. Avoid blocking operations in event handlers
3. Implement appropriate concurrency limits
4. Monitor memory usage

### Debugging

To debug issues:

1. Enable detailed logging
2. Use breakpoints in event handlers
3. Review event bus metrics
4. Check state machine state
