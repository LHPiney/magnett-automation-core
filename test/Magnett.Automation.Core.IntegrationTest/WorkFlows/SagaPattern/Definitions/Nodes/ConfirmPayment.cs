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
/// Try to make payment 
/// </summary>

public class ConfirmPayment(CommonNamedKey name, IEventBus eventBus) : NodeAsync(name, eventBus)
{
    private readonly ContextField<bool> _canMakePayment = ContextField<bool>.WithName("CanMakePayment");
    private readonly ContextField<Payment> _paymentField = ContextField<Payment>.WithName("Payment");

    private static async Task<NodeExit> Confirm(Payment payment)
    {
        await payment.State.DispatchAsync(PaymentDefinition.Action.Confirm);

        return NodeExit.Completed(
            ExitCode.Done,
            $"Payment is Done");
    }
    
    private static async Task<NodeExit> Fail(Payment payment)
    {
       await payment.State.DispatchAsync(PaymentDefinition.Action.Cancel);

        return NodeExit.Failed(
            ExitCode.Failed,
            $"Payment Failed");
    }


    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return NodeExit.Cancelled(
                ExitCode.Done, 
                $"Operation cancelled at {DateTime.UtcNow} ");
        }
        
        var payment = context.Value(_paymentField);

        return context.Value(_canMakePayment)
            ? await Confirm(payment)
            : await Fail(payment);
    }
}