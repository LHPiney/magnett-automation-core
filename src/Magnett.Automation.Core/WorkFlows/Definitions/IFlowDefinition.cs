using Magnett.Automation.Core.WorkFlows.Runtimes;

namespace Magnett.Automation.Core.WorkFlows.Definitions;

public interface IFlowDefinition
{
    public CommonNamedKey InitialNode { get; }
    public bool HasLink(INodeBase sourceNode, string code);        
    public INodeLink GetLink(INodeBase sourceNode, string code);
    public bool HasNode(CommonNamedKey nodeKey);
    public INodeDefinition GetNode(CommonNamedKey nodeKey);
    public INodeDefinition GetNode(INodeBase sourceNode, string code);
}