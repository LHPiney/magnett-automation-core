using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.WorkFlows;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;

public class NodeFake : Node
{
    public NodeFake(string name) : base(name)
    {
    }

    public NodeFake(CommonNamedKey key) : base(key)
    {
    }

    public override NodeExit Execute(Context context)
    {
        throw new System.NotImplementedException();
    }
}