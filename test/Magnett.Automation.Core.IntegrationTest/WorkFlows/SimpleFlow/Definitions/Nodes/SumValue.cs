using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Codes;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Nodes;

internal sealed class SumValue(CommonNamedKey key, IEventBus eventBus) : NodeAsync(key, eventBus)
{
    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return NodeExit.Cancelled(ExitCode.Cancelled);
        }
        
        var firstDigit  = context.Value(ContextDefinition.FirstDigit);
        var secondDigit = context.Value(ContextDefinition.SecondDigit);
            
        await context.StoreAsync(ContextDefinition.Result, firstDigit + secondDigit);

        return NodeExit.Completed(ExitCode.Done.Name);
    }
}