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
/// Check Limit Funds for payment
/// </summary>
public class PreAuthorizePayment(CommonNamedKey name, IEventBus eventBus) : NodeAsync(name, eventBus)
{
    private readonly ContextField<Order> _orderField = ContextField<Order>.WithName("Order");
    private readonly ContextField<double> _creditField = ContextField<double>.WithName("CreditLimit");
    private readonly ContextField<Payment> _paymentField = ContextField<Payment>.WithName("Payment");

    private double _credit;
    private Order _order;
    private Payment _payment;

    private async Task<NodeExit> PreAuthorize()
    {
        await _payment.State.DispatchAsync(PaymentDefinition.Action.PreAuthorize);

        return NodeExit.Completed(
            ExitCode.PreAuthorized,
            $"Payment is pre-authorized, amount of [{_payment.Amount}] credit [{_credit}]");
    }
    
    private async Task<NodeExit> Deny()
    {
        await _payment.State.DispatchAsync(PaymentDefinition.Action.Deny);

        return NodeExit.Failed(
            ExitCode.Denied,
            $"Payment denied, amount of [{_payment.Amount}] credit [{_credit}]");
    }    

    protected override async Task<NodeExit> HandleAsync(Context context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return NodeExit.Cancelled(
                ExitCode.Cancelled,
                $"Operation cancelled at {DateTime.UtcNow} ");
        }
        
        _order = context.Value(_orderField);
        _credit = context.Value(_creditField);
        _payment = await Payment.CreateAsync(_order.Amount, EventBus);

        await context.StoreAsync(_paymentField, _payment);
        await Task.Delay(1000, cancellationToken);

        return _credit >= _order.Amount
            ? await PreAuthorize()
            : await Deny();
    }
}