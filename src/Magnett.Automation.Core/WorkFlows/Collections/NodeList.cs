using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.WorkFlows.Collections
{
    public class NodeList : DictionaryWrapper<INodeBase>
    {
        public static NodeList Create()
        {
            return new NodeList();
        }
    }
}