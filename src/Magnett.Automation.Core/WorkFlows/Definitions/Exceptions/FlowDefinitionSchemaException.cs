namespace Magnett.Automation.Core.WorkFlows.Definitions.Exceptions;

public class FlowDefinitionSchemaException : Exception
{
    public FlowDefinitionSchemaException(string message) : base(message)
    {
    }

    public FlowDefinitionSchemaException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

