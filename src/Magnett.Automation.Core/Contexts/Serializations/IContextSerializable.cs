namespace Magnett.Automation.Core.Contexts.Serializations;

public interface IContextSerializable
{
    ContextFieldSnapshot Serialize(string fieldName, ISerializationContext context);
}

