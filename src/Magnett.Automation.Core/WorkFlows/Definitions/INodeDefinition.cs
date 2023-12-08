using Magnett.Automation.Core.WorkFlows.Runtimes;

namespace Magnett.Automation.Core.WorkFlows.Definitions;

public interface INodeDefinition:  INodeBase
{
    public Type NodeType { get; }
}