namespace Magnett.Automation.Core.WorkFlows.Definitions.Exceptions;

public class InvalidLinkException : FlowDefinitionSchemaException
{
    public string FromNodeKey { get; }
    public string ToNodeKey { get; }
    public string Code { get; }

    public InvalidLinkException(string fromNodeKey, string toNodeKey, string code, string reason) 
        : base($"Invalid link from '{fromNodeKey}' to '{toNodeKey}' with code '{code}': {reason}")
    {
        FromNodeKey = fromNodeKey;
        ToNodeKey = toNodeKey;
        Code = code;
    }
}

