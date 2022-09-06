namespace Magnett.Automation.Core.WorkFlows.Definitions;

public interface INodeDefinition
{
    public Type Type { get; }

    public string Name { get; }
    
    public bool IsSingleton { get; }
}