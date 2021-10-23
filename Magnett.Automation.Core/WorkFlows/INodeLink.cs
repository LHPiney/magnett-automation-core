using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.WorkFlows
{
    public interface INodeLink
    {
        CommonNamedKey Key { get; }
        CommonNamedKey FromNodeKey { get; }
        CommonNamedKey ToNodeKey { get; }
        string Code { get; }
    }
}