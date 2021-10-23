using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.WorkFlows.Collections
{
    internal class NodeList : DictionaryWrapper<INodeBase>
    {
        public static NodeList Create()
        {
            return new NodeList();
        }
    }
}