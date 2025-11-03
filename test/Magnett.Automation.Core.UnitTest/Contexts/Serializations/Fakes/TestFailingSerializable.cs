#nullable enable
using Magnett.Automation.Core.Contexts.Serializations;
using Magnett.Automation.Core.Contexts.Serializations.Attributes;
using System;

namespace Magnett.Automation.Core.UnitTest.Contexts.Serializations.Fakes;

[ContextSerializable]
public class TestFailingSerializable : IContextSerializable
{
    public ContextFieldSnapshot Serialize(string fieldName, ISerializationContext context)
    {
        throw new InvalidOperationException("Serialization failed");
    }
}

