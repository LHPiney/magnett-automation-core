#nullable enable
using Magnett.Automation.Core.Contexts.Serializations;
using Magnett.Automation.Core.Contexts.Serializations.Attributes;
using System.Text.Json;

namespace Magnett.Automation.Core.UnitTest.Contexts.Serializations.Fakes;

[ContextSerializable]
public class TestContextAwareSerializable : IContextSerializable
{
    public string Name { get; private set; } = string.Empty;

    private TestContextAwareSerializable() { }

    public TestContextAwareSerializable(string name)
    {
        Name = name;
    }

    public ContextFieldSnapshot Serialize(string fieldName, ISerializationContext context)
    {
        var data = new TestContextData
        {
            Name = Name,
            HasEventBus = context.EventBus != null,
            HasLogger = context.Logger != null
        };
        return new ContextFieldSnapshot
        {
            FieldName = fieldName,
            FieldType = typeof(TestContextAwareSerializable).AssemblyQualifiedName ?? "",
            Value = null,
            SerializedValue = JsonSerializer.Serialize(data)
        };
    }

    public static object Deserialize(ContextFieldSnapshot snapshot, ISerializationContext context)
    {
        var data = JsonSerializer.Deserialize<TestContextData>(snapshot.SerializedValue!);
        return new TestContextAwareSerializable(data!.Name);
    }
}

