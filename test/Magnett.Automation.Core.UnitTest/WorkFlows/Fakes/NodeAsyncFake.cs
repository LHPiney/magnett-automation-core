using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;

public class NodeAsyncFake : NodeAsync
{
    public NodeAsyncFake(string name, IEventBus eventBus) : base(name, eventBus)
    {
    }

    public NodeAsyncFake(CommonNamedKey key, IEventBus eventBus) : base(key, eventBus)
    {
    }
    
    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        await Task.Delay(100, cancellationToken);

        return NodeExit.Completed("Code");
    }
}