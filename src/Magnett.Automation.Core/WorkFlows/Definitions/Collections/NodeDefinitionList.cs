namespace Magnett.Automation.Core.WorkFlows.Definitions.Collections;

public class NodeDefinitionList : DictionaryWrapper<INodeDefinition>
{
    /// <summary>
    /// Creates a new NodeDefinitionList instance.
    /// </summary>
    /// <returns>A new NodeDefinitionList instance.</returns>
    public static NodeDefinitionList Create()
    {
        return new NodeDefinitionList();
    }
}