namespace Magnett.Automation.Core.WorkFlows.Definitions;

public interface INodeDefinition
{
    public CommonNamedKey Key { get; }
    public string Name => Key?.Name;
    
    public Type NodeType { get; }
}