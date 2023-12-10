using Magnett.Automation.Core.Contexts;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions;

internal static class ContextDefinition
{
    public static ContextField<int> FirstDigit  => ContextField<int>.Create("FieldOne");
    public static ContextField<int> SecondDigit => ContextField<int>.Create("FieldTwo");     
    public static ContextField<int> Result => ContextField<int>.Create("FieldResult");
}