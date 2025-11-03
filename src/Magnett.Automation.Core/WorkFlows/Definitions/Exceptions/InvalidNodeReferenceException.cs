namespace Magnett.Automation.Core.WorkFlows.Definitions.Exceptions;

public class InvalidNodeReferenceException : FlowDefinitionSchemaException
{
    public string NodeKey { get; }

    public InvalidNodeReferenceException(string nodeKey, string reason) 
        : base($"Invalid node reference '{nodeKey}': {reason}")
    {
        NodeKey = nodeKey;
    }
}

