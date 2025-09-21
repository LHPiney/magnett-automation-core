namespace Magnett.Automation.Core.WorkFlows.Runtimes.Collections;

public class NodeList : DictionaryWrapper<INodeBase>
{
    /// <summary>
    /// Creates a new NodeList instance.
    /// </summary>
    /// <returns>A new NodeList instance.</returns>
    public static NodeList Create()
    {
        return new NodeList();
    }
}