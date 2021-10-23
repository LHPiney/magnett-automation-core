using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.WorkFlows.Collections
{
    internal class NodeLinkList : DictionaryWrapper<INodeLink>
    {
        public static NodeLinkList Create()
        {
            return new NodeLinkList();
        }
    }
}