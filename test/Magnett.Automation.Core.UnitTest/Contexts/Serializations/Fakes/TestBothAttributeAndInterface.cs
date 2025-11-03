#nullable enable
using Magnett.Automation.Core.Contexts.Serializations;
using Magnett.Automation.Core.Contexts.Serializations.Attributes;
using System.Text.Json;

namespace Magnett.Automation.Core.UnitTest.Contexts.Serializations.Fakes;

[ContextSerializable]
public class TestBothAttributeAndInterface : IContextSerializable
{
    public string Name { get; private set; } = string.Empty;

    private TestBothAttributeAndInterface() { }

    public TestBothAttributeAndInterface(string name)
    {
        Name = name;
    }

    public ContextFieldSnapshot Serialize(string fieldName, ISerializationContext context)
    {
        var data = new TestContextData { Name = "FromInterface" };
        return new ContextFieldSnapshot
        {
            FieldName = fieldName,
            FieldType = typeof(TestBothAttributeAndInterface).AssemblyQualifiedName ?? "",
            Value = null,
            SerializedValue = JsonSerializer.Serialize(data)
        };
    }

    public static object Deserialize(ContextFieldSnapshot snapshot, ISerializationContext context)
    {
        var data = JsonSerializer.Deserialize<TestContextData>(snapshot.SerializedValue!);
        return new TestBothAttributeAndInterface(data!.Name);
    }
}

