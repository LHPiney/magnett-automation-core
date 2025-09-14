using Magnett.Automation.Core.Contexts;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions;

internal static class ContextDefinition
{
    public static ContextField<int> FirstDigit  => ContextField<int>.WithName("FieldOne");
    public static ContextField<int> SecondDigit => ContextField<int>.WithName("FieldTwo");     
    public static ContextField<int> Result => ContextField<int>.WithName("FieldResult");
}