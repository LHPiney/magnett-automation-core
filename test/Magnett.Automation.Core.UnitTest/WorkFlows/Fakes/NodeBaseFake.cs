using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Runtimes;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;

internal class NodeBaseFake : INodeBase
{
    private NodeBaseFake(string name)
    {
        Key = CommonNamedKey.Create(name);
    }

    public CommonNamedKey Key { get; }

    public static INodeBase Create(string name)
    {
        return new NodeBaseFake(name);
    }
}