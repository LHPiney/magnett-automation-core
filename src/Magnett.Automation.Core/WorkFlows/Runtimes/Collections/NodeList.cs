namespace Magnett.Automation.Core.WorkFlows.Runtimes.Collections;

public class NodeList : DictionaryWrapper<INodeBase>
{
    public static NodeList Create()
    {
        return new NodeList();
    }
}