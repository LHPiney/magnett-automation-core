namespace Magnett.Automation.Core.WorkFlows.Definitions.Exceptions;

public class InvalidFlowDefinitionSchemaException : FlowDefinitionSchemaException
{
    public InvalidFlowDefinitionSchemaException(string message) : base(message)
    {
    }

    public InvalidFlowDefinitionSchemaException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

