namespace Magnett.Automation.Core.WorkFlows.Definitions;

public interface IFlowDefinition
{
    public CommonNamedKey InitialNode { get; }
    public bool HasLink(CommonNamedKey nodeKe, string code);        
    public INodeLink GetLink(CommonNamedKey nodeKe, string code);
    public bool HasNode(CommonNamedKey nodeKey);
    public INodeDefinition GetNode(CommonNamedKey nodeKey);
    public INodeDefinition GetNode(CommonNamedKey nodeKey, string code);
}