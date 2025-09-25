using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Codes;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Entities;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Nodes;

/// <summary>
/// Confirm Order after payment, close transaction
/// </summary>
public class ConfirmOrder(CommonNamedKey name, IEventBus eventBus) : NodeAsync(name, eventBus)
{
    private readonly ContextField<Order> _orderField = ContextField<Order>.WithName("Order");
    
    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return NodeExit.Cancelled(
                ExitCode.Cancelled, 
                "Operation cancelled");
        }
    
        var order = context.Value(_orderField);
        if (order == null)
        {
            return NodeExit.Failed(
                ExitCode.Failed,
                "Order field not found in context");
        }
        
        var confirmedOrder = await order.Confirm();
        
        return NodeExit.Completed(
            ExitCode.Done,
            $"Order confirmed id [{confirmedOrder.Id}] [{confirmedOrder.State}]");
    }
}