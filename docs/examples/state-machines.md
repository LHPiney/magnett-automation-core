---
layout: default
title: State Machine Patterns Example
nav_order: 3
parent: Examples
permalink: /examples/state-machines/
---

# State Machine Patterns Example

This example demonstrates various state machine patterns using Magnett Automation Core, including simple state transitions, async actions, state validation, and complex business logic.

## Overview

We'll explore different state machine patterns with **functional examples** that you can run and test:

- **Document Workflow**: Simple state transitions with validation
- **Payment Processing**: Complex state machine with async actions
- **Order Management**: State machine with conditional transitions
- **User Onboarding**: Multi-step process with error handling

> **📁 Functional Examples**: All examples are available as functional integration tests in `test/Magnett.Automation.Core.IntegrationTest/StateMachines/`. You can run the tests to see the real behavior of each state machine.

## 🚀 Run Examples

To run the functional examples, use the following commands:

```bash
# Run all State Machine examples
dotnet test test/Magnett.Automation.Core.IntegrationTest/ --filter "DocumentMachineTest|PaymentMachineTest|OrderMachineTest|OnboardingMachineTest"

# Run individual examples
dotnet test test/Magnett.Automation.Core.IntegrationTest/ --filter "DocumentMachineTest"
dotnet test test/Magnett.Automation.Core.IntegrationTest/ --filter "PaymentMachineTest"
dotnet test test/Magnett.Automation.Core.IntegrationTest/ --filter "OrderMachineTest"
dotnet test test/Magnett.Automation.Core.IntegrationTest/ --filter "OnboardingMachineTest"
```

### 📂 Estructura de Ejemplos

```
test/Magnett.Automation.Core.IntegrationTest/StateMachines/
├── DocumentMachine/          # Document Processing State Machine
│   ├── DocumentState.cs
│   ├── DocumentAction.cs
│   ├── DocumentStateMachineDefinition.cs
│   ├── DocumentStateMachine.cs
│   ├── Document.cs
│   └── DocumentMachineTest.cs
├── PaymentMachine/           # Payment Processing State Machine
│   ├── PaymentState.cs
│   ├── PaymentAction.cs
│   ├── PaymentStateMachineDefinition.cs
│   ├── PaymentStateMachine.cs
│   ├── Payment.cs
│   └── PaymentMachineTest.cs
├── OrderMachine/             # Order Management State Machine
│   ├── OrderState.cs
│   ├── OrderAction.cs
│   ├── OrderStateMachineDefinition.cs
│   ├── OrderStateMachine.cs
│   ├── Order.cs
│   └── OrderMachineTest.cs
└── OnboardingMachine/        # User Onboarding State Machine
    ├── OnboardingState.cs
    ├── OnboardingAction.cs
    ├── OnboardingStateMachineDefinition.cs
    ├── OnboardingStateMachine.cs
    ├── UserOnboarding.cs
    └── OnboardingMachineTest.cs
```

## Implementation

### 1. Document Processing State Machine

> **📁 View Complete Example**: `test/Magnett.Automation.Core.IntegrationTest/StateMachines/DocumentMachine/`

```csharp
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Builders;
using Magnett.Automation.Core.Commons;

// Define states as enumerations (BEST PRACTICE)
public static class DocumentState : Enumeration
{
    public static readonly DocumentState Draft = new(1, nameof(Draft));
    public static readonly DocumentState UnderReview = new(2, nameof(UnderReview));
    public static readonly DocumentState Approved = new(3, nameof(Approved));
    public static readonly DocumentState Rejected = new(4, nameof(Rejected));
    public static readonly DocumentState Published = new(5, nameof(Published));
    public static readonly DocumentState Archived = new(6, nameof(Archived));
    public static readonly DocumentState Deleted = new(7, nameof(Deleted));

    private DocumentState(int id, string name) : base(id, name) { }
}

// Define actions as enumerations (BEST PRACTICE)
public static class DocumentAction : Enumeration
{
    public static readonly DocumentAction Submit = new(1, nameof(Submit));
    public static readonly DocumentAction Approve = new(2, nameof(Approve));
    public static readonly DocumentAction Reject = new(3, nameof(Reject));
    public static readonly DocumentAction RequestChanges = new(4, nameof(RequestChanges));
    public static readonly DocumentAction Publish = new(5, nameof(Publish));
    public static readonly DocumentAction Archive = new(6, nameof(Archive));
    public static readonly DocumentAction Revise = new(7, nameof(Revise));
    public static readonly DocumentAction Update = new(8, nameof(Update));
    public static readonly DocumentAction Restore = new(9, nameof(Restore));
    public static readonly DocumentAction Delete = new(10, nameof(Delete));

    private DocumentAction(int id, string name) : base(id, name) { }
}

public static class DocumentStateMachineDefinition
{
    public static IMachineDefinition Definition { get; } = MachineDefinitionBuilder.Create()
        .InitialState(DocumentState.Draft)
            .OnAction(DocumentAction.Submit).ToState(DocumentState.UnderReview)
            .OnAction(DocumentAction.Delete).ToState(DocumentState.Deleted)
            .Build()
        .AddState(DocumentState.UnderReview)
            .OnAction(DocumentAction.Approve).ToState(DocumentState.Approved)
            .OnAction(DocumentAction.Reject).ToState(DocumentState.Rejected)
            .OnAction(DocumentAction.RequestChanges).ToState(DocumentState.Draft)
            .Build()
        .AddState(DocumentState.Approved)
            .OnAction(DocumentAction.Publish).ToState(DocumentState.Published)
            .OnAction(DocumentAction.Archive).ToState(DocumentState.Archived)
            .Build()
        .AddState(DocumentState.Rejected)
            .OnAction(DocumentAction.Revise).ToState(DocumentState.Draft)
            .OnAction(DocumentAction.Delete).ToState(DocumentState.Deleted)
            .Build()
        .AddState(DocumentState.Published)
            .OnAction(DocumentAction.Update).ToState(DocumentState.Draft)
            .OnAction(DocumentAction.Archive).ToState(DocumentState.Archived)
            .Build()
        .AddState(DocumentState.Archived)
            .OnAction(DocumentAction.Restore).ToState(DocumentState.Draft)
            .Build()
        .AddState(DocumentState.Deleted)
            .Build()
        .BuildDefinition();
}

public class Document
{
    private IMachine? _machine;
    
    public string Id { get; }
    public string Title { get; }
    public string Content { get; }
    public DocumentState CurrentState => _machine?.CurrentDocumentState ?? DocumentState.Draft;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public string Author { get; }
    public List<string> Reviewers { get; }
    public string? RejectionReason { get; private set; }

    public Document(string id, string title, string content, string author)
    {
        Id = id;
        Title = title;
        Content = content;
        Author = author;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Reviewers = new List<string>();
        RejectionReason = null;
    }

    public async Task InitializeAsync(IEventBus eventBus)
    {
        _machine = await Machine.CreateAsync(DocumentStateMachineDefinition.Definition, eventBus);
        UpdatedAt = DateTime.UtcNow;
    }

    public async Task<bool> SubmitAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Document not initialized");
        
        try
        {
            await _machine.DispatchAsync(DocumentAction.Submit);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ApproveAsync(string reviewer)
    {
        if (_machine == null) throw new InvalidOperationException("Document not initialized");
        
        try
        {
            await _machine.DispatchAsync(DocumentAction.Approve);
            AddReviewer(reviewer);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> RejectAsync(string reviewer, string reason)
    {
        if (_machine == null) throw new InvalidOperationException("Document not initialized");
        
        try
        {
            await _machine.DispatchAsync(DocumentAction.Reject);
            RejectionReason = reason;
            AddReviewer(reviewer);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> PublishAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Document not initialized");
        
        try
        {
            await _machine.DispatchAsync(DocumentAction.Publish);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> RequestChangesAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Document not initialized");
        
        try
        {
            await _machine.DispatchAsync(DocumentAction.RequestChanges);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Document not initialized");
        
        try
        {
            await _machine.DispatchAsync(DocumentAction.Delete);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void AddReviewer(string reviewer)
    {
        if (!Reviewers.Contains(reviewer))
        {
            Reviewers.Add(reviewer);
        }
    }
}
```

### 2. Usage Example

```csharp
public class Program
{
    public static async Task Main(string[] args)
    {
        // Setup logging
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        
        // Create event bus
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();
        
        // Create a document
        var document = new Document("doc-1", "Sample Document", "This is a sample document", "author-1", eventBus);
        Console.WriteLine($"Document created: {document.Id}, State: {document.CurrentState}");
        
        // Initialize the document with its state machine
        await document.InitializeAsync();
        Console.WriteLine($"Document initialized, State: {document.CurrentState}");
        
        // Submit document for review
        var submitted = await document.SubmitAsync();
        Console.WriteLine($"Document submitted: {submitted}, State: {document.CurrentState}");
        
        // Approve document
        var approved = await document.ApproveAsync("reviewer-1");
        Console.WriteLine($"Document approved: {approved}, State: {document.CurrentState}");
        
        // Publish document
        var published = await document.PublishAsync();
        Console.WriteLine($"Document published: {published}, State: {document.CurrentState}");
        
        // Show final state
        Console.WriteLine($"Final document state: {document.CurrentState}");
        Console.WriteLine($"Reviewers: {string.Join(", ", document.Reviewers)}");
        
        // Stop event bus
        await eventBus.StopAsync();
    }
}
```

**Output esperado:**
```
Document created: doc-1, State: Draft
Document initialized, State: Draft
Document submitted: True, State: UnderReview
Document approved: True, State: Approved
Document published: True, State: Published
Final document state: Published
Reviewers: reviewer-1
```

## Best Practices Demonstrated

### ✅ Use Enumerations Instead of String Literals

**❌ Bad Practice (String Literals):**
```csharp
.OnAction("Submit").ToState("UnderReview")  // Error-prone, no IntelliSense
await _machine.DispatchAsync("Submit");     // Typos possible
```

**✅ Good Practice (Enumerations):**
```csharp
.OnAction(DocumentAction.Submit).ToState(DocumentState.UnderReview)  // Type-safe, IntelliSense
await _machine.DispatchAsync(DocumentAction.Submit);                 // Compile-time checking
```

### ✅ Benefits of Using Enumerations:

1. **Type Safety**: Compile-time checking prevents typos
2. **IntelliSense**: IDE autocomplete for states and actions
3. **Refactoring**: Easy to rename states/actions across the codebase
4. **Documentation**: Self-documenting code with clear state/action names
5. **Maintainability**: Centralized definition of all possible states/actions
6. **Testing**: Easy to iterate through all states for comprehensive testing

### ✅ Additional Best Practices:

- **Encapsulate State Machine**: Keep the machine inside the domain entity
- **Initialize Explicitly**: Always call `InitializeAsync()` before using
- **Handle Errors**: Wrap `DispatchAsync()` calls in try-catch blocks
- **Update Timestamps**: Track when state changes occur
- **Validate Transitions**: Let the machine validate valid state transitions
- **Use Events**: Leverage the event bus for audit trails and notifications

### 3. Payment Processing State Machine

> **📁 View Complete Example**: `test/Magnett.Automation.Core.IntegrationTest/StateMachines/PaymentMachine/`

```csharp
// Define payment states as enumerations (BEST PRACTICE)
public static class PaymentState : Enumeration
{
    public static readonly PaymentState Pending = new(1, nameof(Pending));
    public static readonly PaymentState Processing = new(2, nameof(Processing));
    public static readonly PaymentState Completed = new(3, nameof(Completed));
    public static readonly PaymentState Failed = new(4, nameof(Failed));
    public static readonly PaymentState Cancelled = new(5, nameof(Cancelled));
    public static readonly PaymentState Refunded = new(6, nameof(Refunded));

    private PaymentState(int id, string name) : base(id, name) { }
}

// Define payment actions as enumerations (BEST PRACTICE)
public static class PaymentAction : Enumeration
{
    public static readonly PaymentAction Process = new(1, nameof(Process));
    public static readonly PaymentAction Complete = new(2, nameof(Complete));
    public static readonly PaymentAction Fail = new(3, nameof(Fail));
    public static readonly PaymentAction Retry = new(4, nameof(Retry));
    public static readonly PaymentAction Cancel = new(5, nameof(Cancel));
    public static readonly PaymentAction Refund = new(6, nameof(Refund));

    private PaymentAction(int id, string name) : base(id, name) { }
}

public static class PaymentStateMachineDefinition
{
    public static IMachineDefinition Definition { get; } = MachineDefinitionBuilder.Create()
        .InitialState(PaymentState.Pending)
            .OnAction(PaymentAction.Process).ToState(PaymentState.Processing)
            .OnAction(PaymentAction.Cancel).ToState(PaymentState.Cancelled)
            .Build()
        .AddState(PaymentState.Processing)
            .OnAction(PaymentAction.Complete).ToState(PaymentState.Completed)
            .OnAction(PaymentAction.Fail).ToState(PaymentState.Failed)
            .OnAction(PaymentAction.Retry).ToState(PaymentState.Pending)
            .Build()
        .AddState(PaymentState.Completed)
            .OnAction(PaymentAction.Refund).ToState(PaymentState.Refunded)
            .Build()
        .AddState(PaymentState.Failed)
            .OnAction(PaymentAction.Retry).ToState(PaymentState.Pending)
            .OnAction(PaymentAction.Cancel).ToState(PaymentState.Cancelled)
            .Build()
        .AddState(PaymentState.Cancelled)
            .Build()
        .AddState(PaymentState.Refunded)
            .Build()
        .BuildDefinition();
}

public class Payment
{
    private IMachine? _machine;
    
    public string Id { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public string PaymentMethod { get; }
    public PaymentState CurrentState => _machine?.Current.Key.Name == null 
        ? PaymentState.Pending 
        : PaymentState.FromName(_machine.Current.Key.Name);
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public string TransactionId { get; private set; }
    public string ErrorMessage { get; private set; }
    public int RetryCount { get; private set; }

    public Payment(string id, decimal amount, string currency, string paymentMethod)
    {
        Id = id;
        Amount = amount;
        Currency = currency;
        PaymentMethod = paymentMethod;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        RetryCount = 0;
    }

    public async Task InitializeAsync(IEventBus eventBus)
    {
        _machine = await Machine.CreateAsync(PaymentStateMachineDefinition.Definition, eventBus);
        UpdatedAt = DateTime.UtcNow;
    }

    public async Task<bool> ProcessAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Payment not initialized");
        
        try
        {
            await _machine.DispatchAsync(PaymentAction.Process);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CompleteAsync(string transactionId)
    {
        if (_machine == null) throw new InvalidOperationException("Payment not initialized");
        
        try
        {
            await _machine.DispatchAsync(PaymentAction.Complete);
            TransactionId = transactionId;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> FailAsync(string errorMessage)
    {
        if (_machine == null) throw new InvalidOperationException("Payment not initialized");
        
        try
        {
            await _machine.DispatchAsync(PaymentAction.Fail);
            ErrorMessage = errorMessage;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> RetryAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Payment not initialized");
        
        try
        {
            await _machine.DispatchAsync(PaymentAction.Retry);
            RetryCount++;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CancelAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Payment not initialized");
        
        try
        {
            await _machine.DispatchAsync(PaymentAction.Cancel);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> RefundAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Payment not initialized");
        
        try
        {
            await _machine.DispatchAsync(PaymentAction.Refund);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
```

### 4. Payment Processing Usage Example

```csharp
public class PaymentExample
{
    public static async Task RunPaymentExampleAsync()
    {
        // Setup logging
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        
        // Create event bus
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();
        
        // Create a payment
        var payment = new Payment("pay-1", 100.00m, "USD", "CreditCard");
        Console.WriteLine($"Payment created: {payment.Id}, State: {payment.CurrentState}");
        
        // Initialize the payment with its state machine
        await payment.InitializeAsync(eventBus);
        Console.WriteLine($"Payment initialized, State: {payment.CurrentState}");
        
        // Process payment
        var processed = await payment.ProcessAsync();
        Console.WriteLine($"Payment processed: {processed}, State: {payment.CurrentState}");
        
        // Complete payment
        var completed = await payment.CompleteAsync("txn-12345");
        Console.WriteLine($"Payment completed: {completed}, State: {payment.CurrentState}");
        Console.WriteLine($"Transaction ID: {payment.TransactionId}");
        
        // Show final state
        Console.WriteLine($"Final payment state: {payment.CurrentState}");
        
        // Stop event bus
        await eventBus.StopAsync();
    }
}
```

**Output esperado:**
```
Payment created: pay-1, State: Pending
Payment initialized, State: Pending
Payment processed: True, State: Processing
Payment completed: True, State: Completed
Transaction ID: txn-12345
Final payment state: Completed
```

### 5. Order Management State Machine

> **📁 View Complete Example**: `test/Magnett.Automation.Core.IntegrationTest/StateMachines/OrderMachine/`

```csharp
// Define order states as enumerations (BEST PRACTICE)
public static class OrderState : Enumeration
{
    public static readonly OrderState Created = new(1, nameof(Created));
    public static readonly OrderState Validated = new(2, nameof(Validated));
    public static readonly OrderState InventoryReserved = new(3, nameof(InventoryReserved));
    public static readonly OrderState PaymentProcessing = new(4, nameof(PaymentProcessing));
    public static readonly OrderState PaymentCompleted = new(5, nameof(PaymentCompleted));
    public static readonly OrderState PaymentFailed = new(6, nameof(PaymentFailed));
    public static readonly OrderState Shipped = new(7, nameof(Shipped));
    public static readonly OrderState Delivered = new(8, nameof(Delivered));
    public static readonly OrderState Returned = new(9, nameof(Returned));
    public static readonly OrderState Refunded = new(10, nameof(Refunded));
    public static readonly OrderState Cancelled = new(11, nameof(Cancelled));
    public static readonly OrderState Completed = new(12, nameof(Completed));

    private OrderState(int id, string name) : base(id, name) { }
}

// Define order actions as enumerations (BEST PRACTICE)
public static class OrderAction : Enumeration
{
    public static readonly OrderAction Validate = new(1, nameof(Validate));
    public static readonly OrderAction ReserveInventory = new(2, nameof(ReserveInventory));
    public static readonly OrderAction ProcessPayment = new(3, nameof(ProcessPayment));
    public static readonly OrderAction PaymentSuccess = new(4, nameof(PaymentSuccess));
    public static readonly OrderAction PaymentFailed = new(5, nameof(PaymentFailed));
    public static readonly OrderAction Ship = new(6, nameof(Ship));
    public static readonly OrderAction Deliver = new(7, nameof(Deliver));
    public static readonly OrderAction Complete = new(8, nameof(Complete));
    public static readonly OrderAction Return = new(9, nameof(Return));
    public static readonly OrderAction ProcessRefund = new(10, nameof(ProcessRefund));
    public static readonly OrderAction Refund = new(11, nameof(Refund));
    public static readonly OrderAction RetryPayment = new(12, nameof(RetryPayment));
    public static readonly OrderAction ReleaseInventory = new(13, nameof(ReleaseInventory));
    public static readonly OrderAction Cancel = new(14, nameof(Cancel));

    private OrderAction(int id, string name) : base(id, name) { }
}

public static class OrderStateMachineDefinition
{
    public static IMachineDefinition Definition { get; } = MachineDefinitionBuilder.Create()
        .InitialState(OrderState.Created)
            .OnAction(OrderAction.Validate).ToState(OrderState.Validated)
            .OnAction(OrderAction.Cancel).ToState(OrderState.Cancelled)
            .Build()
        .AddState(OrderState.Validated)
            .OnAction(OrderAction.ReserveInventory).ToState(OrderState.InventoryReserved)
            .OnAction(OrderAction.Cancel).ToState(OrderState.Cancelled)
            .Build()
        .AddState(OrderState.InventoryReserved)
            .OnAction(OrderAction.ProcessPayment).ToState(OrderState.PaymentProcessing)
            .OnAction(OrderAction.ReleaseInventory).ToState(OrderState.Validated)
            .OnAction(OrderAction.Cancel).ToState(OrderState.Cancelled)
            .Build()
        .AddState(OrderState.PaymentProcessing)
            .OnAction(OrderAction.PaymentSuccess).ToState(OrderState.PaymentCompleted)
            .OnAction(OrderAction.PaymentFailed).ToState(OrderState.PaymentFailed)
            .Build()
        .AddState(OrderState.PaymentCompleted)
            .OnAction(OrderAction.Ship).ToState(OrderState.Shipped)
            .OnAction(OrderAction.Refund).ToState(OrderState.Refunded)
            .Build()
        .AddState(OrderState.PaymentFailed)
            .OnAction(OrderAction.RetryPayment).ToState(OrderState.PaymentProcessing)
            .OnAction(OrderAction.ReleaseInventory).ToState(OrderState.Validated)
            .OnAction(OrderAction.Cancel).ToState(OrderState.Cancelled)
            .Build()
        .AddState(OrderState.Shipped)
            .OnAction(OrderAction.Deliver).ToState(OrderState.Delivered)
            .OnAction(OrderAction.Return).ToState(OrderState.Returned)
            .Build()
        .AddState(OrderState.Delivered)
            .OnAction(OrderAction.Complete).ToState(OrderState.Completed)
            .OnAction(OrderAction.Return).ToState(OrderState.Returned)
            .Build()
        .AddState(OrderState.Returned)
            .OnAction(OrderAction.ProcessRefund).ToState(OrderState.Refunded)
            .Build()
        .AddState(OrderState.Refunded)
            .Build()
        .AddState(OrderState.Cancelled)
            .Build()
        .AddState(OrderState.Completed)
            .Build()
        .BuildDefinition();
}

public class Order
{
    private IMachine? _machine;
    
    public string Id { get; }
    public string CustomerId { get; }
    public decimal Amount { get; }
    public List<OrderItem> Items { get; }
    public OrderState CurrentState => _machine?.Current.Key.Name == null 
        ? OrderState.Created 
        : OrderState.FromName(_machine.Current.Key.Name);
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public string PaymentTransactionId { get; private set; }
    public string ShippingTrackingNumber { get; private set; }
    public string ErrorMessage { get; private set; }

    public Order(string id, string customerId, decimal amount, List<OrderItem> items)
    {
        Id = id;
        CustomerId = customerId;
        Amount = amount;
        Items = items;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public async Task InitializeAsync(IEventBus eventBus)
    {
        _machine = await Machine.CreateAsync(OrderStateMachineDefinition.Definition, eventBus);
        UpdatedAt = DateTime.UtcNow;
    }

    public async Task<bool> ValidateAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Order not initialized");
        
        try
        {
            await _machine.DispatchAsync(OrderAction.Validate);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ReserveInventoryAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Order not initialized");
        
        try
        {
            await _machine.DispatchAsync(OrderAction.ReserveInventory);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ProcessPaymentAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Order not initialized");
        
        try
        {
            await _machine.DispatchAsync(OrderAction.ProcessPayment);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CompletePaymentAsync(string transactionId)
    {
        if (_machine == null) throw new InvalidOperationException("Order not initialized");
        
        try
        {
            await _machine.DispatchAsync(OrderAction.PaymentSuccess);
            PaymentTransactionId = transactionId;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> FailPaymentAsync(string errorMessage)
    {
        if (_machine == null) throw new InvalidOperationException("Order not initialized");
        
        try
        {
            await _machine.DispatchAsync(OrderAction.PaymentFailed);
            ErrorMessage = errorMessage;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ShipAsync(string trackingNumber)
    {
        if (_machine == null) throw new InvalidOperationException("Order not initialized");
        
        try
        {
            await _machine.DispatchAsync(OrderAction.Ship);
            ShippingTrackingNumber = trackingNumber;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeliverAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Order not initialized");
        
        try
        {
            await _machine.DispatchAsync(OrderAction.Deliver);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CompleteAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Order not initialized");
        
        try
        {
            await _machine.DispatchAsync(OrderAction.Complete);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CancelAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Order not initialized");
        
        try
        {
            await _machine.DispatchAsync(OrderAction.Cancel);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

public class OrderItem
{
    public string ProductId { get; }
    public string ProductName { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }
    public decimal TotalPrice => Quantity * UnitPrice;

    public OrderItem(string productId, string productName, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
```

### 6. Order Management Usage Example

```csharp
public class OrderExample
{
    public static async Task RunOrderExampleAsync()
    {
        // Setup logging
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        
        // Create event bus
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();
        
        // Create order items
        var items = new List<OrderItem>
        {
            new OrderItem("product-1", "Widget", 2, 50.00m),
            new OrderItem("product-2", "Gadget", 1, 75.00m)
        };
        
        // Create an order
        var order = new Order("order-1", "customer-123", 175.00m, items);
        Console.WriteLine($"Order created: {order.Id}, State: {order.CurrentState}");
        
        // Initialize the order with its state machine
        await order.InitializeAsync(eventBus);
        Console.WriteLine($"Order initialized, State: {order.CurrentState}");
        
        // Validate order
        var validated = await order.ValidateAsync();
        Console.WriteLine($"Order validated: {validated}, State: {order.CurrentState}");
        
        // Reserve inventory
        var reserved = await order.ReserveInventoryAsync();
        Console.WriteLine($"Inventory reserved: {reserved}, State: {order.CurrentState}");
        
        // Process payment
        var processed = await order.ProcessPaymentAsync();
        Console.WriteLine($"Payment processed: {processed}, State: {order.CurrentState}");
        
        // Complete payment
        var completed = await order.CompletePaymentAsync("txn-67890");
        Console.WriteLine($"Payment completed: {completed}, State: {order.CurrentState}");
        Console.WriteLine($"Transaction ID: {order.PaymentTransactionId}");
        
        // Ship order
        var shipped = await order.ShipAsync("TRK-12345");
        Console.WriteLine($"Order shipped: {shipped}, State: {order.CurrentState}");
        Console.WriteLine($"Tracking Number: {order.ShippingTrackingNumber}");
        
        // Deliver order
        var delivered = await order.DeliverAsync();
        Console.WriteLine($"Order delivered: {delivered}, State: {order.CurrentState}");
        
        // Complete order
        var finished = await order.CompleteAsync();
        Console.WriteLine($"Order completed: {finished}, State: {order.CurrentState}");
        
        // Show final state
        Console.WriteLine($"Final order state: {order.CurrentState}");
        
        // Stop event bus
        await eventBus.StopAsync();
    }
}
```

**Output esperado:**
```
Order created: order-1, State: Created
Order initialized, State: Created
Order validated: True, State: Validated
Inventory reserved: True, State: InventoryReserved
Payment processed: True, State: PaymentProcessing
Payment completed: True, State: PaymentCompleted
Order shipped: True, State: Shipped
Tracking Number: TRK-12345
Order delivered: True, State: Delivered
Order completed: True, State: Completed
Final order state: Completed
```

### 7. User Onboarding State Machine

> **📁 View Complete Example**: `test/Magnett.Automation.Core.IntegrationTest/StateMachines/OnboardingMachine/`

```csharp
// Define onboarding states as enumerations (BEST PRACTICE)
public static class OnboardingState : Enumeration
{
    public static readonly OnboardingState Registration = new(1, nameof(Registration));
    public static readonly OnboardingState EmailVerification = new(2, nameof(EmailVerification));
    public static readonly OnboardingState ProfileSetup = new(3, nameof(ProfileSetup));
    public static readonly OnboardingState Preferences = new(4, nameof(Preferences));
    public static readonly OnboardingState Welcome = new(5, nameof(Welcome));
    public static readonly OnboardingState Onboarded = new(6, nameof(Onboarded));
    public static readonly OnboardingState Abandoned = new(7, nameof(Abandoned));

    private OnboardingState(int id, string name) : base(id, name) { }
}

// Define onboarding actions as enumerations (BEST PRACTICE)
public static class OnboardingAction : Enumeration
{
    public static readonly OnboardingAction Complete = new(1, nameof(Complete));
    public static readonly OnboardingAction Verify = new(2, nameof(Verify));
    public static readonly OnboardingAction Resend = new(3, nameof(Resend));
    public static readonly OnboardingAction Skip = new(4, nameof(Skip));
    public static readonly OnboardingAction Abandon = new(5, nameof(Abandon));
    public static readonly OnboardingAction Restart = new(6, nameof(Restart));

    private OnboardingAction(int id, string name) : base(id, name) { }
}

public static class UserOnboardingStateMachineDefinition
{
    public static IMachineDefinition Definition { get; } = MachineDefinitionBuilder.Create()
        .InitialState(OnboardingState.Registration)
            .OnAction(OnboardingAction.Complete).ToState(OnboardingState.EmailVerification)
            .OnAction(OnboardingAction.Abandon).ToState(OnboardingState.Abandoned)
            .Build()
        .AddState(OnboardingState.EmailVerification)
            .OnAction(OnboardingAction.Verify).ToState(OnboardingState.ProfileSetup)
            .OnAction(OnboardingAction.Resend).ToState(OnboardingState.EmailVerification)
            .OnAction(OnboardingAction.Abandon).ToState(OnboardingState.Abandoned)
            .Build()
        .AddState(OnboardingState.ProfileSetup)
            .OnAction(OnboardingAction.Complete).ToState(OnboardingState.Preferences)
            .OnAction(OnboardingAction.Skip).ToState(OnboardingState.Preferences)
            .OnAction(OnboardingAction.Abandon).ToState(OnboardingState.Abandoned)
            .Build()
        .AddState(OnboardingState.Preferences)
            .OnAction(OnboardingAction.Complete).ToState(OnboardingState.Welcome)
            .OnAction(OnboardingAction.Skip).ToState(OnboardingState.Welcome)
            .OnAction(OnboardingAction.Abandon).ToState(OnboardingState.Abandoned)
            .Build()
        .AddState(OnboardingState.Welcome)
            .OnAction(OnboardingAction.Complete).ToState(OnboardingState.Onboarded)
            .Build()
        .AddState(OnboardingState.Onboarded)
            .Build()
        .AddState(OnboardingState.Abandoned)
            .OnAction(OnboardingAction.Restart).ToState(OnboardingState.Registration)
            .Build()
        .BuildDefinition();
}

public class UserOnboarding
{
    private IMachine? _machine;
    
    public string UserId { get; }
    public string Email { get; }
    public OnboardingState CurrentState => _machine?.Current.Key.Name == null 
        ? OnboardingState.Registration 
        : OnboardingState.FromName(_machine.Current.Key.Name);
    public DateTime StartedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public bool EmailVerified { get; private set; }
    public bool ProfileCompleted { get; private set; }
    public bool PreferencesCompleted { get; private set; }
    public int VerificationAttempts { get; private set; }

    public UserOnboarding(string userId, string email)
    {
        UserId = userId;
        Email = email;
        StartedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        EmailVerified = false;
        ProfileCompleted = false;
        PreferencesCompleted = false;
        VerificationAttempts = 0;
    }

    public async Task InitializeAsync(IEventBus eventBus)
    {
        _machine = await Machine.CreateAsync(UserOnboardingStateMachineDefinition.Definition, eventBus);
        UpdatedAt = DateTime.UtcNow;
    }

    public async Task<bool> CompleteRegistrationAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.Complete);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> VerifyEmailAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.Verify);
            EmailVerified = true;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ResendVerificationAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.Resend);
            VerificationAttempts++;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CompleteProfileAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.Complete);
            ProfileCompleted = true;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> SkipProfileAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.Skip);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CompletePreferencesAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.Complete);
            PreferencesCompleted = true;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> SkipPreferencesAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.Skip);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CompleteWelcomeAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.Complete);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> AbandonAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.Abandon);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> RestartAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.Restart);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
```

### 8. User Onboarding Usage Example

```csharp
public class OnboardingExample
{
    public static async Task RunOnboardingExampleAsync()
    {
        // Setup logging
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        
        // Create event bus
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();
        
        // Create user onboarding
        var onboarding = new UserOnboarding("user-123", "user@example.com");
        Console.WriteLine($"Onboarding created: {onboarding.UserId}, State: {onboarding.CurrentState}");
        
        // Initialize the onboarding with its state machine
        await onboarding.InitializeAsync(eventBus);
        Console.WriteLine($"Onboarding initialized, State: {onboarding.CurrentState}");
        
        // Complete registration
        var registered = await onboarding.CompleteRegistrationAsync();
        Console.WriteLine($"Registration completed: {registered}, State: {onboarding.CurrentState}");
        
        // Verify email
        var verified = await onboarding.VerifyEmailAsync();
        Console.WriteLine($"Email verified: {verified}, State: {onboarding.CurrentState}");
        Console.WriteLine($"Email verified flag: {onboarding.EmailVerified}");
        
        // Complete profile
        var profileCompleted = await onboarding.CompleteProfileAsync();
        Console.WriteLine($"Profile completed: {profileCompleted}, State: {onboarding.CurrentState}");
        Console.WriteLine($"Profile completed flag: {onboarding.ProfileCompleted}");
        
        // Complete preferences
        var preferencesCompleted = await onboarding.CompletePreferencesAsync();
        Console.WriteLine($"Preferences completed: {preferencesCompleted}, State: {onboarding.CurrentState}");
        Console.WriteLine($"Preferences completed flag: {onboarding.PreferencesCompleted}");
        
        // Complete welcome
        var welcomed = await onboarding.CompleteWelcomeAsync();
        Console.WriteLine($"Welcome completed: {welcomed}, State: {onboarding.CurrentState}");
        
        // Show final state
        Console.WriteLine($"Final onboarding state: {onboarding.CurrentState}");
        Console.WriteLine($"Total onboarding time: {DateTime.UtcNow - onboarding.StartedAt}");
        
        // Stop event bus
        await eventBus.StopAsync();
    }
}
```

**Output esperado:**
```
Onboarding created: user-123, State: Registration
Onboarding initialized, State: Registration
Registration completed: True, State: EmailVerification
Email verified: True, State: ProfileSetup
Email verified flag: True
Profile completed: True, State: Preferences
Profile completed flag: True
Preferences completed: True, State: Welcome
Preferences completed flag: True
Welcome completed: True, State: Onboarded
Final onboarding state: Onboarded
Total onboarding time: 00:00:00.1234567
```

## Summary

These examples demonstrate how to use Magnett Automation Core state machines effectively:

1. **Document Processing**: Shows a complete document lifecycle with review and approval
2. **Payment Processing**: Demonstrates payment flow with retry and error handling
3. **Order Management**: Illustrates complex e-commerce order processing
4. **User Onboarding**: Shows multi-step user registration and setup

### ✅ Best Practices Implemented

Each example follows best practices:
- ✅ **Type-safe enumerations** instead of string literals
- ✅ **Encapsulated state machines** within domain entities
- ✅ **Explicit initialization** before use
- ✅ **Error handling** with try-catch blocks
- ✅ **Timestamp tracking** for state changes
- ✅ **Clear method names** that reflect business operations
- ✅ **Switch expressions** for clean state determination
- ✅ **Comprehensive test coverage** with integration tests

### 🔄 Improvements Implemented

The functional examples include significant improvements over static snippets:

#### **🏗️ Better Encapsulation**
- Before: State conversion logic in the domain entity
- After: Logic encapsulated in `{Entity}StateMachine.Current{Entity}State`

#### **🔒 Stronger Type Safety**
- Before: Error-prone string literals
- After: Type-safe `Enumeration` with IntelliSense

#### **🧹 Cleaner Code**
- Before: Nested ternary operators
- After: Elegant and readable `switch` expressions

#### **🧪 Comprehensive Tests**
- Before: Static examples without validation
- After: Integration tests validating real behavior

### 🧪 Testing and Validation

All examples include comprehensive integration tests that validate:
- **Complete workflows** from start to finish
- **Error scenarios** and edge cases
- **Invalid transitions** handling
- **State consistency** throughout the process
- **Event bus integration** for audit trails

### 🚀 Next Steps

These patterns can be adapted for any domain that requires state management and workflow orchestration. The functional examples serve as:
- **Reference implementations** for your own state machines
- **Test cases** to validate your understanding
- **Starting points** for more complex workflows
- **Documentation** of real-world usage patterns

**Run the tests to see these patterns in action!**