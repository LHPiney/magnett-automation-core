#nullable enable
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Contexts.Serializations;
using Magnett.Automation.Core.Contexts.Serializations.Attributes;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.WorkFlows.Serialization;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Contexts;

public class ContextSerializationTest
{
    private readonly Mock<IFlowCodec> _mockCodec;
    private readonly Mock<IEventBus> _mockEventBus;

    public ContextSerializationTest()
    {
        _mockCodec = new Mock<IFlowCodec>();
        _mockEventBus = new Mock<IEventBus>();
    }

    [Fact]
    public async Task GetSnapshot_WithDefaultCodec_ShouldReturnValidSnapshot()
    {
        var context = Context.Create();
        var stringField = ContextField<string>.WithName("StringField");
        var intField = ContextField<int>.WithName("IntField");

        await context.StoreAsync(stringField, "TestValue");
        await context.StoreAsync(intField, 42);

        var snapshot = context.GetSnapshot();

        Assert.NotNull(snapshot);
        Assert.Equal(context.Id, snapshot.Id);
        Assert.Equal(context.CreatedAt, snapshot.CreatedAt);
        Assert.Equal(2, snapshot.Fields.Count);
    }

    [Fact]
    public async Task GetSnapshot_WithCustomCodec_ShouldUseProvidedCodec()
    {
        var context = Context.Create();
        var field = ContextField<TestComplexType>.WithName("TestField");
        var complexValue = new TestComplexType { Id = 999, Name = "CodecTest" };
        await context.StoreAsync(field, complexValue);

        var serializedBytes = Encoding.UTF8.GetBytes("{\"id\":999,\"name\":\"CodecTest\"}");
        _mockCodec.Setup(codec => codec.Serialize(It.IsAny<object>()))
                  .Returns(serializedBytes.AsMemory());

        var snapshot = context.GetSnapshot(_mockCodec.Object);

        Assert.NotNull(snapshot);
        _mockCodec.Verify(codec => codec.Serialize(It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task GetSnapshot_WithComplexType_ShouldSerializeUsingCodec()
    {
        var context = Context.Create();
        var field = ContextField<TestComplexType>.WithName("ComplexField");
        var complexValue = new TestComplexType { Id = 123, Name = "Test" };
        
        await context.StoreAsync(field, complexValue);

        var serializedJson = JsonSerializer.Serialize(complexValue);
        var serializedBytes = Encoding.UTF8.GetBytes(serializedJson);
        
        _mockCodec.Setup(codec => codec.Serialize(complexValue))
                  .Returns(serializedBytes.AsMemory());

        var snapshot = context.GetSnapshot(_mockCodec.Object);

        Assert.NotNull(snapshot);
        Assert.Single(snapshot.Fields);
        
        var fieldSnapshot = snapshot.Fields[0];
        Assert.Equal("ComplexField", fieldSnapshot.FieldName);
        Assert.Equal(typeof(TestComplexType).AssemblyQualifiedName, fieldSnapshot.FieldType);
        Assert.Null(fieldSnapshot.Value);
        Assert.NotNull(fieldSnapshot.SerializedValue);
    }

    [Fact]
    public async Task GetSnapshot_WithSimpleTypes_ShouldStoreInValueProperty()
    {
        var context = Context.Create();
        var stringField = ContextField<string>.WithName("StringField");
        var intField = ContextField<int>.WithName("IntField");
        var boolField = ContextField<bool>.WithName("BoolField");
        var dateField = ContextField<DateTime>.WithName("DateField");

        await context.StoreAsync(stringField, "TestString");
        await context.StoreAsync(intField, 42);
        await context.StoreAsync(boolField, true);
        await context.StoreAsync(dateField, DateTime.UtcNow);

        var snapshot = context.GetSnapshot();

        Assert.Equal(4, snapshot.Fields.Count);

        foreach (var fieldSnapshot in snapshot.Fields)
        {
            Assert.NotNull(fieldSnapshot.Value);
            Assert.Null(fieldSnapshot.SerializedValue);
        }
    }

    [Fact]
    public async Task FromSnapshot_WithValidSnapshot_ShouldRestoreContext()
    {
        var originalContext = Context.Create();
        var stringField = ContextField<string>.WithName("StringField");
        var intField = ContextField<int>.WithName("IntField");

        await originalContext.StoreAsync(stringField, "TestValue");
        await originalContext.StoreAsync(intField, 42);

        var snapshot = originalContext.GetSnapshot();
        var restoredContext = Context.FromSnapshot(snapshot);

        Assert.NotNull(restoredContext);
        Assert.Equal("TestValue", restoredContext.Value(stringField));
        Assert.Equal(42, restoredContext.Value(intField));
    }

    [Fact]
    public void FromSnapshot_WithComplexType_ShouldDeserializeCorrectly()
    {
        var originalValue = new TestComplexType { Id = 123, Name = "Test" };
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        
        var snapshot = new ContextSnapshot
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Fields =
            [
                new ContextFieldSnapshot
                {
                    FieldName = "ComplexField",
                    FieldType = typeof(TestComplexType).AssemblyQualifiedName,
                    Value = null,
                    SerializedValue = JsonSerializer.Serialize(originalValue, jsonOptions)
                }
            ]
        };

        var restoredContext = Context.FromSnapshot(snapshot);
        var field = ContextField<TestComplexType>.WithName("ComplexField");
        var restoredValue = restoredContext.Value(field);

        Assert.NotNull(restoredValue);
        Assert.Equal(originalValue.Id, restoredValue!.Id);
        Assert.Equal(originalValue.Name, restoredValue.Name);
    }

    [Fact]
    public void FromSnapshot_WithEventBus_ShouldCreateContextWithEventBus()
    {
        var snapshot = new ContextSnapshot
        {                                                                           
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Fields = new List<ContextFieldSnapshot>()
        };

        var restoredContext = Context.FromSnapshot(snapshot, null, _mockEventBus.Object);

        Assert.NotNull(restoredContext);
        Assert.Equal(snapshot.Id, restoredContext.Id);
    }

    [Fact]
    public async Task GetSnapshot_WithIncompatibleComplexType_ShouldSkipSerialization()
    {
        var context = Context.Create();
        var field = ContextField<TestIncompatibleType>.WithName("IncompatibleField");
        var incompatibleValue = new TestIncompatibleType(_mockEventBus.Object);

        await context.StoreAsync(field, incompatibleValue);

        var snapshot = context.GetSnapshot();

        Assert.NotNull(snapshot);
        Assert.Single(snapshot.Fields);

        var fieldSnapshot = snapshot.Fields[0];
        Assert.Equal("IncompatibleField", fieldSnapshot.FieldName);
        Assert.Null(fieldSnapshot.Value);
        Assert.Null(fieldSnapshot.SerializedValue);
    }

    [Fact]
    public void FromSnapshot_WithIncompatibleType_ShouldReturnNullForField()
    {
        var snapshot = new ContextSnapshot
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Fields = new List<ContextFieldSnapshot>
            {
                new()
                {
                    FieldName = "IncompatibleField",
                    FieldType = typeof(TestIncompatibleType).AssemblyQualifiedName,
                    Value = null,
                    SerializedValue = null
                }
            }
        };

        var restoredContext = Context.FromSnapshot(snapshot);
        var field = ContextField<TestIncompatibleType>.WithName("IncompatibleField");

        Assert.Null(restoredContext.Value(field));
    }

    [Fact]
    public async Task GetSnapshot_WithNullValues_ShouldHandleGracefully()
    {
        var context = Context.Create();
        var stringField = ContextField<string?>.WithName("NullStringField");
        var intField = ContextField<int?>.WithName("NullIntField");

        await context.StoreAsync(stringField, null);
        await context.StoreAsync(intField, null);

        var snapshot = context.GetSnapshot();

        Assert.NotNull(snapshot);
        Assert.Equal(2, snapshot.Fields.Count);

        foreach (var fieldSnapshot in snapshot.Fields)
        {
            Assert.Null(fieldSnapshot.Value);
            Assert.Null(fieldSnapshot.SerializedValue);
        }
    }

    [Fact]
    public void FromSnapshot_WithNullValues_ShouldRestoreNulls()
    {
        var snapshot = new ContextSnapshot
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Fields = new List<ContextFieldSnapshot>
            {
                new()
                {
                    FieldName = "NullStringField",
                    FieldType = typeof(string).AssemblyQualifiedName,
                    Value = null,
                    SerializedValue = null
                },
                new()
                {
                    FieldName = "NullIntField",
                    FieldType = typeof(int?).AssemblyQualifiedName,
                    Value = null,
                    SerializedValue = null
                }
            }
        };

        var restoredContext = Context.FromSnapshot(snapshot);
        var stringField = ContextField<string?>.WithName("NullStringField");
        var intField = ContextField<int?>.WithName("NullIntField");

        Assert.Null(restoredContext.Value(stringField));
        Assert.Null(restoredContext.Value(intField));
    }

    [Fact]
    public async Task GetSnapshot_AndFromSnapshot_ShouldBeRoundTripCompatible()
    {
        var originalContext = Context.Create();
        var stringField = ContextField<string>.WithName("StringField");
        var intField = ContextField<int>.WithName("IntField");
        var boolField = ContextField<bool>.WithName("BoolField");
        var dateField = ContextField<DateTime>.WithName("DateField");
        var complexField = ContextField<TestComplexType>.WithName("ComplexField");

        var testDate = DateTime.UtcNow;
        var complexValue = new TestComplexType { Id = 456, Name = "RoundTripTest" };

        await originalContext.StoreAsync(stringField, "RoundTripValue");
        await originalContext.StoreAsync(intField, 789);
        await originalContext.StoreAsync(boolField, false);
        await originalContext.StoreAsync(dateField, testDate);
        await originalContext.StoreAsync(complexField, complexValue);

        var snapshot = originalContext.GetSnapshot();
        var restoredContext = Context.FromSnapshot(snapshot);

        Assert.Equal("RoundTripValue", restoredContext.Value(stringField));
        Assert.Equal(789, restoredContext.Value(intField));
        Assert.False(restoredContext.Value(boolField));
        Assert.Equal(testDate, restoredContext.Value(dateField));
        Assert.Equal(complexValue.Id, restoredContext.Value(complexField)!.Id);
        Assert.Equal(complexValue.Name, restoredContext.Value(complexField)!.Name);
    }

    [Fact]
    public void GetSnapshot_WithEmptyContext_ShouldReturnEmptyFields()
    {
        var context = Context.Create();
        var snapshot = context.GetSnapshot();

        Assert.NotNull(snapshot);
        Assert.Empty(snapshot.Fields);
        Assert.Equal(context.Id, snapshot.Id);
        Assert.Equal(context.CreatedAt, snapshot.CreatedAt);
    }

    [Fact]
    public void FromSnapshot_WithEmptyFields_ShouldCreateEmptyContext()
    {
        var snapshot = new ContextSnapshot
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Fields = new List<ContextFieldSnapshot>()
        };

        var restoredContext = Context.FromSnapshot(snapshot);

        Assert.NotNull(restoredContext);
        Assert.Equal(snapshot.Id, restoredContext.Id);
        Assert.Equal(snapshot.CreatedAt, restoredContext.CreatedAt);
    }
 
    [Fact]
    public async Task GetSnapshot_WithAdditionalSimpleTypes_ShouldStoreInValueProperty()
    {
        var context = Context.Create();
        var decimalField = ContextField<decimal>.WithName("DecimalField");
        var timeSpanField = ContextField<TimeSpan>.WithName("TimeSpanField");
        var dateTimeOffsetField = ContextField<DateTimeOffset>.WithName("DateTimeOffsetField");
        var uriField = ContextField<Uri>.WithName("UriField");

        var testUri = new Uri("https://example.com");
        var testTimeSpan = TimeSpan.FromHours(2.5);
        var testDateTimeOffset = DateTimeOffset.UtcNow;

        await context.StoreAsync(decimalField, 123.456m);
        await context.StoreAsync(timeSpanField, testTimeSpan);
        await context.StoreAsync(dateTimeOffsetField, testDateTimeOffset);
        await context.StoreAsync(uriField, testUri);

        var snapshot = context.GetSnapshot();

        Assert.Equal(4, snapshot.Fields.Count);

        foreach (var fieldSnapshot in snapshot.Fields)
        {
            Assert.NotNull(fieldSnapshot.Value);
            Assert.Null(fieldSnapshot.SerializedValue);
        }
    }

    [Fact]
    public async Task GetSnapshot_AndFromSnapshot_WithAdditionalSimpleTypes_ShouldBeRoundTripCompatible()
    {
        var originalContext = Context.Create();
        var decimalField = ContextField<decimal>.WithName("DecimalField");
        var timeSpanField = ContextField<TimeSpan>.WithName("TimeSpanField");
        var dateTimeOffsetField = ContextField<DateTimeOffset>.WithName("DateTimeOffsetField");
        var uriField = ContextField<Uri>.WithName("UriField");

        var testDecimal = 987.654m;
        var testUri = new Uri("https://test.com/path");
        var testTimeSpan = TimeSpan.FromMinutes(123);
        var testDateTimeOffset = new DateTimeOffset(2025, 1, 15, 10, 30, 0, TimeSpan.FromHours(1));

        await originalContext.StoreAsync(decimalField, testDecimal);
        await originalContext.StoreAsync(timeSpanField, testTimeSpan);
        await originalContext.StoreAsync(dateTimeOffsetField, testDateTimeOffset);
        await originalContext.StoreAsync(uriField, testUri);

        var snapshot = originalContext.GetSnapshot();
        var restoredContext = Context.FromSnapshot(snapshot);

        Assert.Equal(testDecimal, restoredContext.Value(decimalField));
        Assert.Equal(testTimeSpan, restoredContext.Value(timeSpanField));
        Assert.Equal(testDateTimeOffset, restoredContext.Value(dateTimeOffsetField));
        Assert.Equal(testUri.ToString(), restoredContext.Value(uriField)!.ToString());
    }

    [Fact]
    public async Task GetSnapshot_WithContextSerializableAttribute_ShouldSerializeSuccessfully()
    {
        var context = Context.Create();
        var field = ContextField<AttributedType>.WithName("AttributedField");
        var value = new AttributedType
        {
            Id = 100,
            Name = "TestAttributed",
            Description = "Test description"
        };

        await context.StoreAsync(field, value);
        var snapshot = context.GetSnapshot();

        Assert.NotNull(snapshot);
        Assert.Single(snapshot.Fields);
        var fieldSnapshot = snapshot.Fields[0];
        Assert.NotNull(fieldSnapshot.SerializedValue);
    }

    [Fact]
    public async Task GetSnapshot_AndFromSnapshot_WithContextSerializable_ShouldRoundTrip()
    {
        var originalContext = Context.Create();
        var field = ContextField<AttributedType>.WithName("AttributedField");
        var originalValue = new AttributedType
        {
            Id = 200,
            Name = "RoundTrip",
            Description = "Round trip test"
        };

        await originalContext.StoreAsync(field, originalValue);
        var snapshot = originalContext.GetSnapshot();
        var restoredContext = Context.FromSnapshot(snapshot);
        var restoredValue = restoredContext.Value(field);

        Assert.NotNull(restoredValue);
        Assert.Equal(originalValue.Id, restoredValue.Id);
        Assert.Equal(originalValue.Name, restoredValue.Name);
        Assert.Equal(originalValue.Description, restoredValue.Description);
    }

    [Fact]
    public async Task GetSnapshot_WithContextIgnore_ShouldSkipIgnoredProperties()
    {
        var context = Context.Create();
        var field = ContextField<TypeWithIgnoredProperties>.WithName("IgnoreTest");
        var value = new TypeWithIgnoredProperties
        {
            IncludedProperty = "This should be serialized",
            IgnoredProperty = "This should NOT be serialized",
            IgnoredWithReason = "Also should NOT be serialized"
        };

        await context.StoreAsync(field, value);
        var snapshot = context.GetSnapshot();
        var restoredContext = Context.FromSnapshot(snapshot);
        var restored = restoredContext.Value(field);

        Assert.NotNull(restored);
        Assert.Equal("This should be serialized", restored.IncludedProperty);
        // Ignored properties should be null or default after deserialization
        Assert.Null(restored.IgnoredProperty);
        Assert.Null(restored.IgnoredWithReason);
    }

    [Fact]
    public async Task GetSnapshot_WithJsonIgnore_ShouldSkipJsonIgnoredProperties()
    {
        var context = Context.Create();
        var field = ContextField<TypeWithJsonIgnore>.WithName("JsonIgnoreTest");
        var value = new TypeWithJsonIgnore
        {
            SerializableProperty = "Include this",
            JsonIgnoredProperty = "Skip this"
        };

        await context.StoreAsync(field, value);
        var snapshot = context.GetSnapshot();
        var restoredContext = Context.FromSnapshot(snapshot);
        var restored = restoredContext.Value(field);

        Assert.NotNull(restored);
        Assert.Equal("Include this", restored.SerializableProperty);
        Assert.Null(restored.JsonIgnoredProperty);
    }

    [Fact]
    public async Task GetSnapshot_WithNestedContextSerializable_ShouldSerializeNested()
    {
        var context = Context.Create();
        var field = ContextField<ParentType>.WithName("NestedTest");
        var value = new ParentType
        {
            ParentId = 1,
            Child = new ChildType
            {
                ChildId = 2,
                ChildName = "Nested"
            }
        };

        await context.StoreAsync(field, value);
        var snapshot = context.GetSnapshot();
        var restoredContext = Context.FromSnapshot(snapshot);
        var restored = restoredContext.Value(field);

        Assert.NotNull(restored);
        Assert.Equal(1, restored.ParentId);
        Assert.NotNull(restored.Child);
        Assert.Equal(2, restored.Child.ChildId);
        Assert.Equal("Nested", restored.Child.ChildName);
    }

    [Fact]
    public async Task GetSnapshot_WithComputedProperty_ShouldIgnoreReadOnlyProperty()
    {
        var context = Context.Create();
        var field = ContextField<TypeWithComputedProperty>.WithName("ComputedTest");
        var value = new TypeWithComputedProperty
        {
            FirstName = "John",
            LastName = "Doe"
            // FullName is computed, should be ignored
        };

        await context.StoreAsync(field, value);
        var snapshot = context.GetSnapshot();
        var restoredContext = Context.FromSnapshot(snapshot);
        var restored = restoredContext.Value(field);

        Assert.NotNull(restored);
        Assert.Equal("John", restored.FirstName);
        Assert.Equal("Doe", restored.LastName);
        // FullName should be recomputed
        Assert.Equal("John Doe", restored.FullName);
    }
}

[ContextSerializable]
public class AttributedType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

[ContextSerializable]
public class TypeWithIgnoredProperties
{
    public string IncludedProperty { get; set; } = string.Empty;

    [ContextIgnore]
    [JsonIgnore]
    public string? IgnoredProperty { get; set; }

    [ContextIgnore(Reason = "Test reason")]
    [JsonIgnore]
    public string? IgnoredWithReason { get; set; }
}

[ContextSerializable]
public class TypeWithJsonIgnore
{
    public string SerializableProperty { get; set; } = string.Empty;

    [JsonIgnore]
    public string? JsonIgnoredProperty { get; set; }
}

[ContextSerializable]
public class ParentType
{
    public int ParentId { get; set; }
    public ChildType? Child { get; set; }
}

[ContextSerializable]
public class ChildType
{
    public int ChildId { get; set; }
    public string ChildName { get; set; } = string.Empty;
}

[ContextSerializable]
public class TypeWithComputedProperty
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // Read-only computed property - should be automatically ignored
    public string FullName => $"{FirstName} {LastName}";
}

public class TestComplexType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TestIncompatibleType
{
    public TestIncompatibleType(IEventBus eventBus)
    {
        EventBus = eventBus;
    }

    public IEventBus EventBus { get; }
}
