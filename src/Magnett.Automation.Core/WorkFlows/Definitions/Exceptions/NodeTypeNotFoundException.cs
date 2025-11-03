namespace Magnett.Automation.Core.WorkFlows.Definitions.Exceptions;

public class NodeTypeNotFoundException : FlowDefinitionSchemaException
{
    public string NodeKey { get; }
    public string NodeTypeName { get; }

    public NodeTypeNotFoundException(string nodeKey, string nodeTypeName) 
        : base($"Node type '{nodeTypeName}' not found for node '{nodeKey}'")
    {
        NodeKey = nodeKey;
        NodeTypeName = nodeTypeName;
    }
}

