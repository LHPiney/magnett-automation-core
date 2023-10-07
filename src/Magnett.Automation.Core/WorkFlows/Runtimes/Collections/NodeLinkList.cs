namespace Magnett.Automation.Core.WorkFlows.Runtimes.Collections;

public class NodeLinkList : DictionaryWrapper<INodeLink>
{
    public static NodeLinkList Create()
    {
        return new NodeLinkList();
    }
}