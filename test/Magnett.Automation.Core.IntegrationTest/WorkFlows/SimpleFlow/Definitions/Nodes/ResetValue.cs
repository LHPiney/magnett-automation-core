using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Codes;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Nodes;

internal sealed class ResetValue(CommonNamedKey key, IEventBus eventBus) : NodeAsync(key, eventBus)
{
    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        var tasks = new Task[]
        {
            context.StoreAsync(ContextDefinition.FirstDigit, 0),
            context.StoreAsync(ContextDefinition.SecondDigit, 0),
            context.StoreAsync(ContextDefinition.Result, 0),
        };
        
        await Task.WhenAll(tasks);
            
        return NodeExit.Completed(ExitCode.Ok);
    }
}