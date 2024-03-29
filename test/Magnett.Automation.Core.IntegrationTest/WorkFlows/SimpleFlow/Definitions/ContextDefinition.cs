﻿using Magnett.Automation.Core.Contexts;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions;

internal class ContextDefinition
{
    public ContextField<int> FirstDigit  { get; }
    public ContextField<int> SecondDigit { get; }      
    public ContextField<int> Result      { get; }

    private ContextDefinition()
    {
        FirstDigit  =  ContextField<int>.Create("FieldOne");           
        SecondDigit =  ContextField<int>.Create("FieldTwo");           
        Result      =  ContextField<int>.Create("FieldResult");
    }

    public static ContextDefinition Create()
    {
        return new ContextDefinition();
    }
}