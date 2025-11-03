#nullable enable
using Magnett.Automation.Core.Contexts.Serializations;
using Magnett.Automation.Core.Contexts.Serializations.Attributes;
using System;

namespace Magnett.Automation.Core.UnitTest.Contexts.Serializations.Fakes;

[ContextSerializable]
public class TestFailingDeserializable : IContextSerializable
{
    public ContextFieldSnapshot Serialize(string fieldName, ISerializationContext context)
    {
        return new ContextFieldSnapshot
        {
            FieldName = fieldName,
            FieldType = typeof(TestFailingDeserializable).AssemblyQualifiedName ?? "",
            Value = null,
            SerializedValue = "test"
        };
    }

    public static object Deserialize(ContextFieldSnapshot snapshot, ISerializationContext context)
    {
        throw new InvalidOperationException("Deserialization failed");
    }
}

