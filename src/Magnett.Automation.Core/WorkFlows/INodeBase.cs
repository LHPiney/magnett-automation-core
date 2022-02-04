using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;

namespace Magnett.Automation.Core.WorkFlows
{
    public interface INodeBase  
    {
        public CommonNamedKey Key { get; }
        public string Name => Key?.Name;
    }
}