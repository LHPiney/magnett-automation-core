#nullable enable
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Contexts.Serializations;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.WorkFlows.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Magnett.Automation.Core.IntegrationTest.Contexts;

public class ContextSerializationIntegrationTest
{
    [Fact]
    public void ContextSerialization_WithSimpleTypes_ShouldWorkEndToEnd()
    {
        // Arrange
        var originalContext = Context.Create();
        var stringField = ContextField<string>.WithName("StringField");
        var intField = ContextField<int>.WithName("IntField");
        var boolField = ContextField<bool>.WithName("BoolField");
        var dateField = ContextField<DateTime>.WithName("DateField");

        var testDate = DateTime.UtcNow;

        // Act - Store values
        originalContext.StoreAsync(stringField, "TestValue").Wait();
        originalContext.StoreAsync(intField, 42).Wait();
        originalContext.StoreAsync(boolField, true).Wait();
        originalContext.StoreAsync(dateField, testDate).Wait();

        // Act - Serialize
        var snapshot = originalContext.GetSnapshot();

        // Act - Deserialize
        var restoredContext = Context.FromSnapshot(snapshot);

        // Assert
        Assert.NotNull(snapshot);
        Assert.Equal(originalContext.Id, snapshot.Id);
        Assert.Equal(originalContext.CreatedAt, snapshot.CreatedAt);
        Assert.Equal(4, snapshot.Fields.Count);

        Assert.NotNull(restoredContext);
        Assert.Equal(originalContext.Id, restoredContext.Id);
        Assert.Equal(originalContext.CreatedAt, restoredContext.CreatedAt);

        Assert.Equal("TestValue", restoredContext.Value(stringField));
        Assert.Equal(42, restoredContext.Value(intField));
        Assert.True(restoredContext.Value(boolField));
        Assert.Equal(testDate, restoredContext.Value(dateField));
    }

    [Fact]
    public void ContextSerialization_WithComplexType_ShouldWorkEndToEnd()
    {
        // Arrange
        var originalContext = Context.Create();
        var complexField = ContextField<TestComplexType>.WithName("ComplexField");
        var originalValue = new TestComplexType { Id = 123, Name = "TestComplex" };

        // Act - Store value
        originalContext.StoreAsync(complexField, originalValue).Wait();

        // Act - Serialize
        var snapshot = originalContext.GetSnapshot();

        // Act - Deserialize
        var restoredContext = Context.FromSnapshot(snapshot);

        // Assert
        Assert.NotNull(snapshot);
        Assert.Single(snapshot.Fields);
        
        var fieldSnapshot = snapshot.Fields[0];
        Assert.Equal("ComplexField", fieldSnapshot.FieldName);
        Assert.Equal(typeof(TestComplexType).AssemblyQualifiedName, fieldSnapshot.FieldType);
        Assert.Null(fieldSnapshot.Value); // Complex types should be in SerializedValue
        Assert.NotNull(fieldSnapshot.SerializedValue);

        Assert.NotNull(restoredContext);
        var restoredValue = restoredContext.Value(complexField);
        Assert.NotNull(restoredValue);
        Assert.Equal(originalValue.Id, restoredValue!.Id);
        Assert.Equal(originalValue.Name, restoredValue.Name);
    }

    [Fact]
    public void ContextSerialization_WithNullValues_ShouldWorkEndToEnd()
    {
        // Arrange
        var originalContext = Context.Create();
        var stringField = ContextField<string?>.WithName("NullStringField");
        var intField = ContextField<int?>.WithName("NullIntField");

        // Act - Store null values
        originalContext.StoreAsync(stringField, null).Wait();
        originalContext.StoreAsync(intField, null).Wait();

        // Act - Serialize
        var snapshot = originalContext.GetSnapshot();

        // Act - Deserialize
        var restoredContext = Context.FromSnapshot(snapshot);

        // Assert
        Assert.NotNull(snapshot);
        Assert.Equal(2, snapshot.Fields.Count);

        Assert.NotNull(restoredContext);
        Assert.Null(restoredContext.Value(stringField));
        Assert.Null(restoredContext.Value(intField));
    }

    [Fact]
    public void ContextSerialization_WithMixedTypes_ShouldWorkEndToEnd()
    {
        // Arrange
        var originalContext = Context.Create();
        var stringField = ContextField<string>.WithName("StringField");
        var intField = ContextField<int>.WithName("IntField");
        var complexField = ContextField<TestComplexType>.WithName("ComplexField");
        var nullField = ContextField<string?>.WithName("NullField");

        var complexValue = new TestComplexType { Id = 456, Name = "MixedTest" };

        // Act - Store mixed values
        originalContext.StoreAsync(stringField, "MixedValue").Wait();
        originalContext.StoreAsync(intField, 789).Wait();
        originalContext.StoreAsync(complexField, complexValue).Wait();
        originalContext.StoreAsync(nullField, null).Wait();

        // Act - Serialize
        var snapshot = originalContext.GetSnapshot();

        // Act - Deserialize
        var restoredContext = Context.FromSnapshot(snapshot);

        // Assert
        Assert.NotNull(snapshot);
        Assert.Equal(4, snapshot.Fields.Count);

        Assert.NotNull(restoredContext);
        Assert.Equal("MixedValue", restoredContext.Value(stringField));
        Assert.Equal(789, restoredContext.Value(intField));
        
        var restoredComplex = restoredContext.Value(complexField);
        Assert.NotNull(restoredComplex);
        Assert.Equal(complexValue.Id, restoredComplex!.Id);
        Assert.Equal(complexValue.Name, restoredComplex.Name);
        
        Assert.Null(restoredContext.Value(nullField));
    }

    [Fact]
    public void ContextSerialization_WithEventBus_ShouldPreserveEventBus()
    {
        // Arrange
        var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<Magnett.Automation.Core.Events.Implementations.EventBus>.Instance;
        var eventBus = Magnett.Automation.Core.Events.Implementations.EventBus.Create(logger);
        var originalContext = Context.Create(eventBus);
        var stringField = ContextField<string>.WithName("EventField");

        // Act - Store value
        originalContext.StoreAsync(stringField, "EventValue").Wait();

        // Act - Serialize
        var snapshot = originalContext.GetSnapshot();

        // Act - Deserialize with same event bus
        var restoredContext = Context.FromSnapshot(snapshot, null, eventBus);

        // Assert
        Assert.NotNull(restoredContext);
        Assert.Equal("EventValue", restoredContext.Value(stringField));
        Assert.NotNull(restoredContext.EventStream);
    }

    [Fact]
    public void ContextSerialization_EmptyContext_ShouldWorkEndToEnd()
    {
        // Arrange
        var originalContext = Context.Create();

        // Act - Serialize empty context
        var snapshot = originalContext.GetSnapshot();

        // Act - Deserialize
        var restoredContext = Context.FromSnapshot(snapshot);

        // Assert
        Assert.NotNull(snapshot);
        Assert.Empty(snapshot.Fields);
        Assert.Equal(originalContext.Id, snapshot.Id);
        Assert.Equal(originalContext.CreatedAt, snapshot.CreatedAt);

        Assert.NotNull(restoredContext);
        Assert.Equal(originalContext.Id, restoredContext.Id);
        Assert.Equal(originalContext.CreatedAt, restoredContext.CreatedAt);
    }

    [Fact]
    public void ContextSerialization_RoundTrip_ShouldPreserveAllData()
    {
        // Arrange
        var originalContext = Context.Create();
        var stringField = ContextField<string>.WithName("StringField");
        var intField = ContextField<int>.WithName("IntField");
        var boolField = ContextField<bool>.WithName("BoolField");
        var dateField = ContextField<DateTime>.WithName("DateField");
        var complexField = ContextField<TestComplexType>.WithName("ComplexField");
        var nullField = ContextField<string?>.WithName("NullField");

        var testDate = DateTime.UtcNow;
        var complexValue = new TestComplexType { Id = 999, Name = "RoundTripTest" };

        // Act - Store all values
        originalContext.StoreAsync(stringField, "RoundTripValue").Wait();
        originalContext.StoreAsync(intField, 111).Wait();
        originalContext.StoreAsync(boolField, false).Wait();
        originalContext.StoreAsync(dateField, testDate).Wait();
        originalContext.StoreAsync(complexField, complexValue).Wait();
        originalContext.StoreAsync(nullField, null).Wait();

        // Act - Round trip
        var snapshot = originalContext.GetSnapshot();
        var restoredContext = Context.FromSnapshot(snapshot);

        // Assert - All values preserved
        Assert.Equal("RoundTripValue", restoredContext.Value(stringField));
        Assert.Equal(111, restoredContext.Value(intField));
        Assert.False(restoredContext.Value(boolField));
        Assert.Equal(testDate, restoredContext.Value(dateField));
        
        var restoredComplex = restoredContext.Value(complexField);
        Assert.NotNull(restoredComplex);
        Assert.Equal(complexValue.Id, restoredComplex!.Id);
        Assert.Equal(complexValue.Name, restoredComplex.Name);
        
        Assert.Null(restoredContext.Value(nullField));

        // Assert - Metadata preserved
        Assert.Equal(originalContext.Id, restoredContext.Id);
        Assert.Equal(originalContext.CreatedAt, restoredContext.CreatedAt);
    }

    [Fact]
    public void ContextSerialization_WithCustomCodec_ShouldUseProvidedCodec()
    {
        // Arrange
        var originalContext = Context.Create();
        var stringField = ContextField<string>.WithName("CustomField");
        var customCodec = FlowCodecFactory.Get("json");

        // Act - Store value
        originalContext.StoreAsync(stringField, "CustomValue").Wait();

        // Act - Serialize with custom codec
        var snapshot = originalContext.GetSnapshot(customCodec);

        // Act - Deserialize with custom codec
        var restoredContext = Context.FromSnapshot(snapshot, customCodec);

        // Assert
        Assert.NotNull(snapshot);
        Assert.NotNull(restoredContext);
        Assert.Equal("CustomValue", restoredContext.Value(stringField));
    }
}

public class TestComplexType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
