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