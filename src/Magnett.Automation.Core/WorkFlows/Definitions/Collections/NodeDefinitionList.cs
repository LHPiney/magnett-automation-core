namespace Magnett.Automation.Core.WorkFlows.Definitions.Collections;

public class NodeDefinitionList : DictionaryWrapper<INodeDefinition>
{
    public static NodeDefinitionList Create()
    {
        return new NodeDefinitionList();
    }
}