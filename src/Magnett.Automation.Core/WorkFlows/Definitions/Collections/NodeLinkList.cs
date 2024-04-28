namespace Magnett.Automation.Core.WorkFlows.Definitions.Collections;

public class NodeLinkList : DictionaryWrapper<INodeLink>
{
    public static NodeLinkList Create()
    {
        return new NodeLinkList();
    }
}