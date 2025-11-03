namespace Magnett.Automation.Core.WorkFlows.Definitions.Exceptions;

public class MissingRequiredFieldException : InvalidFlowDefinitionSchemaException
{
    public string FieldName { get; }

    public MissingRequiredFieldException(string fieldName) 
        : base($"Required field '{fieldName}' is missing")
    {
        FieldName = fieldName;
    }
}

