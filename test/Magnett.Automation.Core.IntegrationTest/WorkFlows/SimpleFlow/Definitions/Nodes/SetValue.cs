using System;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Codes;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Nodes;

internal sealed class SetValue(CommonNamedKey key, IEventBus eventBus) : NodeAsync(key, eventBus)
{
    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return NodeExit.Cancelled(ExitCode.Cancelled);
        }
        
        // Simulate some work
        await Task.Delay(1000, cancellationToken);
        
        // Store random values in the context
        var random = new Random();
        
        var tasks = new Task[]
        {
            context.StoreAsync(ContextDefinition.FirstDigit, random.Next(1000)),
            context.StoreAsync(ContextDefinition.SecondDigit, random.Next(1000))
        };
        
        await Task.WhenAll(tasks);

        return NodeExit.Completed(ExitCode.Assigned.Name);
    }
}