namespace Magnett.Automation.Core.Contexts.Serializations;

public class ContextFieldSnapshot
{
    public string FieldName { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public object? Value { get; set; }
    public string? SerializedValue { get; set; }
}