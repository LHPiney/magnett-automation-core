using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.Contexts.Events;

public record OnChangeFieldValueEvent(
    string Name, 
    string Caller, 
    string FieldName,
    Type   ValueType,
    object Value,
    object PreviousValue) : Event(Name, Caller)
{
    /// <summary>
    /// Creates a new OnChangeFieldValueEvent instance for the specified field value change.
    /// </summary>
    /// <param name="fieldName">The name of the field that changed.</param>
    /// <param name="valueType">The type of the field value.</param>
    /// <param name="value">The new field value.</param>
    /// <param name="previousValue">The previous field value.</param>
    /// <param name="callerName">The name of the caller method.</param>
    /// <returns>A new OnChangeFieldValueEvent instance.</returns>
    public static OnChangeFieldValueEvent Create(
        string fieldName,
        Type   valueType,
        object value,
        object previousValue,
        [CallerMemberName] string callerName = "")
    {
        return new OnChangeFieldValueEvent(
            nameof(OnChangeFieldValueEvent),
            callerName,
            fieldName, 
            valueType,
            value,
            previousValue);
    }
}