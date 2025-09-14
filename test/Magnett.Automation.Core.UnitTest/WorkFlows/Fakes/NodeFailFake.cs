using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;

public class NodeFailFake : Node
{
    public NodeFailFake(string name, IEventBus eventBus) : base(name, eventBus)
    {
    }

    public NodeFailFake(CommonNamedKey key, IEventBus eventBus) : base(key, eventBus)
    {
    }

    protected override NodeExit Handle(Context context)
    {
        return NodeExit.Failed("ERR", "Simulated failure");
    }
}
