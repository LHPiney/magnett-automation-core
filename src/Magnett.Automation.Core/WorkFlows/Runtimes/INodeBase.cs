namespace Magnett.Automation.Core.WorkFlows.Runtimes;

public interface INodeBase  
{
    public CommonNamedKey Key { get; }
    public string Name => Key?.Name;
}