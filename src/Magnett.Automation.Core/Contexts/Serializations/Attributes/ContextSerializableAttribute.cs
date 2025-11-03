namespace Magnett.Automation.Core.Contexts.Serializations.Attributes;

/// <summary>
/// Marks a type as compatible with context serialization.
/// Types marked with this attribute must have a parameterless constructor
/// and contain only serializable properties (or properties marked with [ContextIgnore]).
/// This attribute improves performance by caching validation results.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
public sealed class ContextSerializableAttribute : Attribute
{
}
