namespace Magnett.Automation.Core.Contexts.Serializations.Attributes;

/// <summary>
/// Marks a property to be excluded from context serialization.
/// Properties marked with this attribute will not be included in snapshots.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class ContextIgnoreAttribute : Attribute
{
    /// <summary>
    /// Optional reason for ignoring this property.
    /// Used for documentation and logging purposes.
    /// </summary>
    public string? Reason { get; set; }
}
