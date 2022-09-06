using Magnett.Automation.Core.WorkFlows.Definitions.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Definitions.Builders;

public class NodeDefinitionBuilder : INodeDefinitionBuilder     
{
    private string _nodeName;

    public NodeDefinitionBuilder(string nodeName)
    {
        if (string.IsNullOrEmpty(nodeName))
        {
            throw new ArgumentNullException(nameof(nodeName));
        }

        _nodeName = nodeName;
    }
    
    public static INodeDefinitionBuilder Create(string nodeName)
    {
        return new NodeDefinitionBuilder(nodeName);
    }
}