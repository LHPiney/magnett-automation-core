using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.WorkFlows;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;

public class NodeAsyncFake : NodeAsync
{
    public NodeAsyncFake(string name) : base(name)
    {
    }

    public NodeAsyncFake(CommonNamedKey key) : base(key)
    {
    }
    
    public override async Task<NodeExit> Execute(Context context)
    {
        await Task.Delay(100);

        return NodeExit.Create("Code");
    }
}