namespace Magnett.Automation.Core.WorkFlows.Definitions.Collections;

public class NodeLinkList : DictionaryWrapper<INodeLink>
{
    /// <summary>
    /// Creates a new NodeLinkList instance.
    /// </summary>
    /// <returns>A new NodeLinkList instance.</returns>
    public static NodeLinkList Create()
    {
        return new NodeLinkList();
    }
}