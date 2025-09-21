using System;
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
/// Completed order record, begin transaction
/// </summary>
public class CreateOrder(CommonNamedKey name, IEventBus eventBus) : NodeAsync(name, eventBus)
{
    private readonly ContextField<Order> _orderField = ContextField<Order>.WithName("Order");
    private readonly ContextField<double> _amountField = ContextField<double>.WithName("Amount");
    private readonly ContextField<string> _descriptionField = ContextField<string>.WithName("Description");

    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return NodeExit.Cancelled(
                ExitCode.Done, 
                $"Operation cancelled at {DateTime.UtcNow} ");
        }
        
        var order = await Order.CreateAsync(
            context.Value(_amountField), 
            context.Value(_descriptionField),
            EventBus);

        await order.Validate();

        await context.StoreAsync(_orderField, order);
        await Task.Delay(1000, cancellationToken);
        
        return NodeExit.Completed(
            ExitCode.Created, 
            $"Order id [{order.Id}] [{order.State.Current.Key}]");
    }
}