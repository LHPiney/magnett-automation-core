#nullable enable
using Magnett.Automation.Core.Contexts.Serializations;
using Magnett.Automation.Core.Contexts.Serializations.Attributes;
using System;
using System.Text.Json;

namespace Magnett.Automation.Core.UnitTest.Contexts.Serializations.Fakes;

[ContextSerializable]
public class TestCustomSerializable : IContextSerializable
{
    public string Name { get; private set; }
    public int Value { get; private set; }
    public bool WasDeserialized { get; private set; }

    private TestCustomSerializable()
    {
        WasDeserialized = false;
    }

    public TestCustomSerializable(string name, int value)
    {
        Name = name;
        Value = value;
        WasDeserialized = false;
    }

    public ContextFieldSnapshot Serialize(string fieldName, ISerializationContext context)
    {
        var data = new { Name, Value };
        return new ContextFieldSnapshot
        {
            FieldName = fieldName,
            FieldType = typeof(TestCustomSerializable).AssemblyQualifiedName ?? "",
            Value = null,
            SerializedValue = JsonSerializer.Serialize(data)
        };
    }

    public static object Deserialize(ContextFieldSnapshot snapshot, ISerializationContext context)
    {
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(snapshot.SerializedValue!);
        return new TestCustomSerializable
        {
            Name = jsonElement!.GetProperty(nameof(Name)).GetString() ?? string.Empty,
            Value = jsonElement.GetProperty(nameof(Value)).GetInt32(),
            WasDeserialized = true
        };
    }
}

