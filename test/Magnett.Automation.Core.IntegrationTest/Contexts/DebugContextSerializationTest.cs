#nullable enable
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Contexts.Serializations;
using Magnett.Automation.Core.WorkFlows.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Magnett.Automation.Core.IntegrationTest.Contexts;

public class DebugContextSerializationTest
{
    [Fact]
    public void Debug_SimpleStringSerialization()
    {
        // Arrange
        var context = Context.Create();
        var stringField = ContextField<string>.WithName("TestString");
        
        // Act - Store value
        context.StoreAsync(stringField, "TestValue").Wait();
        
        // Debug - Check what's in the context
        var storedValue = context.Value(stringField);
        Assert.Equal("TestValue", storedValue);
        
        // Act - Create snapshot
        var snapshot = context.GetSnapshot();
        
        // Debug - Check snapshot
        Assert.NotNull(snapshot);
        Assert.Single(snapshot.Fields);
        
        var fieldSnapshot = snapshot.Fields[0];
        Assert.Equal("TestString", fieldSnapshot.FieldName);
        Assert.Equal(typeof(string).AssemblyQualifiedName, fieldSnapshot.FieldType);
        Assert.Equal("TestValue", fieldSnapshot.Value);
        Assert.Null(fieldSnapshot.SerializedValue);
        
        // Act - Restore from snapshot
        var restoredContext = Context.FromSnapshot(snapshot);
        
        // Debug - Check restored context
        Assert.NotNull(restoredContext);
        var restoredValue = restoredContext.Value(stringField);
        
        // This is where it fails
        Assert.Equal("TestValue", restoredValue);
    }
    
    [Fact]
    public void Debug_ComplexTypeSerialization()
    {
        // Arrange
        var context = Context.Create();
        var complexField = ContextField<DebugTestComplexType>.WithName("ComplexField");
        var originalValue = new DebugTestComplexType { Id = 123, Name = "TestComplex" };
        
        // Act - Store value
        context.StoreAsync(complexField, originalValue).Wait();
        
        // Debug - Check what's in the context
        var storedValue = context.Value(complexField);
        Assert.NotNull(storedValue);
        Assert.Equal(123, storedValue!.Id);
        Assert.Equal("TestComplex", storedValue.Name);
        
        // Act - Create snapshot
        var snapshot = context.GetSnapshot();
        
        // Debug - Check snapshot
        Assert.NotNull(snapshot);
        Assert.Single(snapshot.Fields);
        
        var fieldSnapshot = snapshot.Fields[0];
        Assert.Equal("ComplexField", fieldSnapshot.FieldName);
        Assert.Equal(typeof(DebugTestComplexType).AssemblyQualifiedName, fieldSnapshot.FieldType);
        Assert.Null(fieldSnapshot.Value); // Complex types should be in SerializedValue
        Assert.NotNull(fieldSnapshot.SerializedValue);
        Assert.Contains("TestComplex", fieldSnapshot.SerializedValue);
        
        // Act - Restore from snapshot
        var restoredContext = Context.FromSnapshot(snapshot);
        
        // Debug - Check restored context   
        Assert.NotNull(restoredContext);
        var restoredValue = restoredContext.Value(complexField);
        
        // This is where it fails
        Assert.NotNull(restoredValue);
        Assert.Equal(123, restoredValue!.Id);
        Assert.Equal("TestComplex", restoredValue.Name);
    }
    
    [Fact]
    public void Debug_CodecBehavior()
    {
        // Arrange
        var codec = FlowCodecFactory.Get("json");
        var testValue = new DebugTestComplexType { Id = 456, Name = "CodecTest" };
        
        // Act - Serialize
        var bytes = codec.Serialize(testValue);
        var jsonString = System.Text.Encoding.UTF8.GetString(bytes.ToArray());
        
        // Debug - Check serialization
        Assert.NotNull(jsonString);
        Assert.Contains("CodecTest", jsonString);
        
        // Act - Deserialize
        var restoredValue = codec.Deserialize<DebugTestComplexType>(bytes.Span);
        
        // Debug - Check deserialization
        Assert.NotNull(restoredValue);
        Assert.Equal(456, restoredValue!.Id);
        Assert.Equal("CodecTest", restoredValue.Name);
    }
    
    [Fact]
    public void Debug_TypeCompatibility()
    {
        // Test if DebugTestComplexType is considered compatible
        var type = typeof(DebugTestComplexType);
        
        // Check if it has parameterless constructor
        var constructors = type.GetConstructors();
        var hasParameterlessConstructor = constructors.Any(c => c.GetParameters().Length == 0);
        Assert.True(hasParameterlessConstructor);
        
        // Check properties
        var properties = type.GetProperties();
        Assert.Equal(2, properties.Length);
        
        foreach (var prop in properties)
        {
            Assert.True(prop.CanRead);
            Assert.True(prop.CanWrite);
        }
    }
    
    [Fact]
    public void Debug_IsContextCompatibleType()
    {
        // Test if DebugTestComplexType is considered compatible by IsContextCompatibleType
        var type = typeof(DebugTestComplexType);
        
        // Check if it has parameterless constructor
        var constructors = type.GetConstructors();
        var hasParameterlessConstructor = constructors.Any(c => c.GetParameters().Length == 0);
        Assert.True(hasParameterlessConstructor);
        
        // Check properties
        var properties = type.GetProperties();
        Assert.Equal(2, properties.Length);
        
        foreach (var prop in properties)
        {
            Assert.True(prop.CanRead);
            Assert.True(prop.CanWrite);
        }
        
        // Test the actual IsContextCompatibleType logic
        var context = Context.Create();
        var complexField = ContextField<DebugTestComplexType>.WithName("TestComplex");
        var complexValue = new DebugTestComplexType { Id = 999, Name = "CompatibilityTest" };
        
        context.StoreAsync(complexField, complexValue).Wait();
        var snapshot = context.GetSnapshot();
        
        // Check if the field was serialized
        var fieldSnapshot = snapshot.Fields.FirstOrDefault(f => f.FieldName == "TestComplex");
        Assert.NotNull(fieldSnapshot);
        Assert.Equal(typeof(DebugTestComplexType).AssemblyQualifiedName, fieldSnapshot.FieldType);
        Assert.NotNull(fieldSnapshot.SerializedValue);
        
        // Debug - Check the serialized JSON
        Assert.NotNull(fieldSnapshot.SerializedValue);
        Assert.Contains("999", fieldSnapshot.SerializedValue);
        Assert.Contains("CompatibilityTest", fieldSnapshot.SerializedValue);
        
        // Try to deserialize
        var restoredContext = Context.FromSnapshot(snapshot);
        var restoredValue = restoredContext.Value(complexField);
        
        Assert.NotNull(restoredValue);
        Assert.Equal(999, restoredValue.Id);
        Assert.Equal("CompatibilityTest", restoredValue.Name);
    }
}

public class DebugTestComplexType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
