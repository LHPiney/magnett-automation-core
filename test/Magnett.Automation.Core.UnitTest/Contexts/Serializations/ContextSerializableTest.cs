#nullable enable
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Contexts.Serializations;
using Magnett.Automation.Core.UnitTest.Contexts.Serializations.Fakes;
using Magnett.Automation.Core.Contexts.Serializations.Implementations;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Contexts.Serializations;

public class ContextSerializableTest
{
    private readonly ILogger _logger;
    private readonly IEventBus _eventBus;

    public ContextSerializableTest()
    {
        var busLogger = NullLogger<EventBus>.Instance;
        _logger = NullLogger<ContextSerializableTest>.Instance;
        _eventBus = EventBus.Create(busLogger);
    }

    [Fact]
    public async Task GetSnapshot_WithIContextSerializable_ShouldCallSerializeMethod()
    {
        var context = Context.Create(_eventBus);
        var field = ContextField<TestCustomSerializable>.WithName("CustomField");
        var customValue = new TestCustomSerializable("TestValue", 123);
        
        await context.StoreAsync(field, customValue);

        var snapshot = context.GetSnapshot(logger: _logger);

        Assert.NotNull(snapshot);
        Assert.Single(snapshot.Fields);
        
        var fieldSnapshot = snapshot.Fields[0];
        Assert.Equal("CustomField", fieldSnapshot.FieldName);
        Assert.Equal(typeof(TestCustomSerializable).AssemblyQualifiedName, fieldSnapshot.FieldType);
        Assert.Null(fieldSnapshot.Value);
        Assert.NotNull(fieldSnapshot.SerializedValue);
        Assert.Contains("TestValue", fieldSnapshot.SerializedValue);
        Assert.Contains("123", fieldSnapshot.SerializedValue);
    }

    [Fact]
    public async Task FromSnapshot_WithIContextSerializable_ShouldCallStaticDeserializeMethod()
    {
        var originalContext = Context.Create(_eventBus);
        var field = ContextField<TestCustomSerializable>.WithName("CustomField");
        var originalValue = new TestCustomSerializable("DeserializeTest", 456);
        
        await originalContext.StoreAsync(field, originalValue);

        var snapshot = originalContext.GetSnapshot(logger: _logger);
        var restoredContext = Context.FromSnapshot(snapshot, eventBus: _eventBus, logger: _logger);

        Assert.NotNull(restoredContext);
        var restoredValue = restoredContext.Value(field);
        
        Assert.NotNull(restoredValue);
        Assert.Equal("DeserializeTest", restoredValue.Name);
        Assert.Equal(456, restoredValue.Value);
        Assert.True(restoredValue.WasDeserialized);
    }

    [Fact]
    public async Task GetSnapshot_WithIContextSerializable_ShouldPassSerializationContext()
    {
        var context = Context.Create(_eventBus);
        var field = ContextField<TestContextAwareSerializable>.WithName("ContextAwareField");
        var customValue = new TestContextAwareSerializable("Test");
        
        await context.StoreAsync(field, customValue);

        var snapshot = context.GetSnapshot(logger: _logger);

        Assert.NotNull(snapshot);
        Assert.Single(snapshot.Fields);
        
        var fieldSnapshot = snapshot.Fields[0];
        Assert.NotNull(fieldSnapshot.SerializedValue);
        
        var deserialized = JsonSerializer.Deserialize<TestContextData>(fieldSnapshot.SerializedValue!);
        Assert.NotNull(deserialized);
        Assert.True(deserialized!.HasEventBus);
        Assert.True(deserialized.HasLogger);
    }

    [Fact]
    public async Task GetSnapshot_WhenSerializeThrowsException_ShouldContinueWithoutBreaking()
    {
        var context = Context.Create(_eventBus);
        var field1 = ContextField<TestFailingSerializable>.WithName("FailingField");
        var field2 = ContextField<string>.WithName("WorkingField");
        
        await context.StoreAsync(field1, new TestFailingSerializable());
        await context.StoreAsync(field2, "WorkingValue");

        var snapshot = context.GetSnapshot(logger: _logger);

        Assert.NotNull(snapshot);
        Assert.Equal(2, snapshot.Fields.Count);
        
        var workingField = snapshot.Fields.Find(f => f.FieldName == "WorkingField");
        Assert.NotNull(workingField);
        Assert.Equal("WorkingValue", workingField.Value);
    }

    [Fact]
    public async Task FromSnapshot_WhenDeserializeThrowsException_ShouldContinueWithoutBreaking()
    {
        var context = Context.Create(_eventBus);
        var field1 = ContextField<string>.WithName("WorkingField");
        var field2 = ContextField<int>.WithName("IntField");
        
        await context.StoreAsync(field1, "TestValue");
        await context.StoreAsync(field2, 42);

        var snapshot = context.GetSnapshot(logger: _logger);
        
        var failingFieldSnapshot = new ContextFieldSnapshot
        {
            FieldName = "FailingField",
            FieldType = typeof(TestFailingDeserializable).AssemblyQualifiedName ?? "",
            Value = null,
            SerializedValue = "invalid"
        };
        snapshot.Fields.Add(failingFieldSnapshot);

        var restoredContext = Context.FromSnapshot(snapshot, eventBus: _eventBus, logger: _logger);

        Assert.NotNull(restoredContext);
        Assert.Equal("TestValue", restoredContext.Value(field1));
        Assert.Equal(42, restoredContext.Value(field2));
    }

    [Fact]
    public async Task GetSnapshot_WithIContextSerializable_ShouldPrioritizeOverAttribute()
    {
        var context = Context.Create(_eventBus);
        var field = ContextField<TestBothAttributeAndInterface>.WithName("BothField");
        var customValue = new TestBothAttributeAndInterface("InterfaceValue");
        
        await context.StoreAsync(field, customValue);

        var snapshot = context.GetSnapshot(logger: _logger);

        Assert.NotNull(snapshot);
        var fieldSnapshot = snapshot.Fields[0];
        Assert.NotNull(fieldSnapshot.SerializedValue);
        
        var deserialized = JsonSerializer.Deserialize<TestContextData>(fieldSnapshot.SerializedValue!);
        Assert.NotNull(deserialized);
        Assert.Equal("FromInterface", deserialized!.Name);
    }

    [Fact]
    public void SerializationContext_ShouldProvideEventBus()
    {
        var context = new SerializationContext(_eventBus, _logger, null);

        Assert.NotNull(context.EventBus);
        Assert.Equal(_eventBus, context.EventBus);
    }

    [Fact]
    public void SerializationContext_ShouldProvideLogger()
    {
        var context = new SerializationContext(_eventBus, _logger, null);

        Assert.NotNull(context.Logger);
        Assert.Equal(_logger, context.Logger);
    }

    [Fact]
    public void SerializationContext_ShouldAllowNullServices()
    {
        var context = new SerializationContext(null, null, null);

        Assert.Null(context.EventBus);
        Assert.Null(context.Logger);
        Assert.Null(context.ServiceProvider);
    }
}

