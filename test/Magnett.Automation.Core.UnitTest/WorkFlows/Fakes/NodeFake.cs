using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;

public class NodeFake : Node
{
    public NodeFake(string name, IEventBus eventBus) : base(name, eventBus)
    {
    }

    public NodeFake(CommonNamedKey key, IEventBus eventBus) : base(key, eventBus)
    {
    }

    protected override NodeExit Handle(Context context)
    {
        return NodeExit.Completed("ok");
    }
}