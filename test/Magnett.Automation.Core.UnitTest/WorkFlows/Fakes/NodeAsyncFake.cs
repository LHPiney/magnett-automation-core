using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.WorkFlows;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;

public class NodeAsyncFake : INodeAsync
{
    public CommonNamedKey Key { get; } = CommonNamedKey.Create("Fake_Node");
    
    public async Task<NodeExit> Execute(Context context)
    {
        await Task.Delay(100);

        return NodeExit.Create("Code");
    }
}