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
/// Cancel payment and rollback pre-authorization
/// </summary>
public class CancelPayment(CommonNamedKey name, IEventBus eventBus) : NodeAsync(name, eventBus)
{
    private readonly ContextField<Payment> _paymentField = ContextField<Payment>.WithName("Payment");
    
    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return NodeExit.Cancelled(
                ExitCode.Cancelled, 
                $"Operation cancelled at {DateTime.UtcNow} ");
        }
        
        var payment = context.Value(_paymentField);
        if (payment == null)
        {
            return NodeExit.Failed(
                ExitCode.Failed,
                "Payment field not found in context");
        }
        
        var cancelledPayment = await payment.Cancel();
        
        return NodeExit.Completed(
            ExitCode.Done,
            $"Payment cancelled id [{cancelledPayment.Id}] [{cancelledPayment.State}]");
    }
}