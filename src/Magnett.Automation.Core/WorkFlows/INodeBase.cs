namespace Magnett.Automation.Core.WorkFlows;

public interface INodeBase  
{
    public CommonNamedKey Key { get; }
    public string Name => Key?.Name;
}