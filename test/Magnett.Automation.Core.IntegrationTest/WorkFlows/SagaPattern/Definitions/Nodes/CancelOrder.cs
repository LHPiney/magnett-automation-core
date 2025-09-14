using System;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Codes;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Entities;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Nodes;

/// <summary>
///  Rollback Order operation and cancel a process
/// </summary>
public class CancelOrder(CommonNamedKey name, IEventBus eventBus) : NodeAsync(name, eventBus)
{
    private readonly ContextField<Order> _orderField = ContextField<Order>.WithName("Order");
    
    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return NodeExit.Cancelled(
                ExitCode.Done, 
                $"Operation cancelled at {DateTime.UtcNow} ");
        }
        
        var order = await context.Value(_orderField)
            .Cancel();
        
        return NodeExit.Completed(
            ExitCode.Done, 
            $"Order cancelled id [{order.Id}] [{order.State.Current}]");
    }
}