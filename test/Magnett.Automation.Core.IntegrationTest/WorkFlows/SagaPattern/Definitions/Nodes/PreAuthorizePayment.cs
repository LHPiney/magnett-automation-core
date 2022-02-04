using System;
using System.Threading.Tasks;

using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.WorkFlows;
using Magnett.Automation.Core.WorkFlows.Implementations;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Entities;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Nodes;

/// <summary>
/// Check Limit Funds for payment
/// </summary>
public class PreAuthorizePayment : NodeAsync
{
    private readonly ContextField<Order> _orderField = ContextField<Order>.Create("Order");
    private readonly ContextField<double> _creditField = ContextField<double>.Create("CreditLimit");
    private readonly ContextField<Payment> _paymentField = ContextField<Payment>.Create("Payment");

    private  double _credit;
    private  Order _order;
    private  Payment _payment;

    private PreAuthorizePayment(string name) : base(name)
    {

    }

    #region ExitCodes

    public class ExitCode : Enumeration
    {
        public static readonly ExitCode PreAuthorized = new ExitCode(1, nameof(PreAuthorized));
        public static readonly ExitCode Denied = new ExitCode(2, nameof(Denied));

        private ExitCode(int id, string name) : base(id, name)
        {
        }
    }

    #endregion

    private NodeExit PreAuthorize()
    {
        _payment.State.Dispatch(PaymentStateDefinition.Action.PreAuthorize);

        return NodeExit.Create(
            ExitCode.PreAuthorized,
            false,
            $"Payment is pre-authorized, amount of [{_payment.Amount}] credit [{_credit}]");
    }
    
    private NodeExit Deny()
    {
        _payment.State.Dispatch(PaymentStateDefinition.Action.Deny);

        return NodeExit.Create(
            ExitCode.Denied,
            true,
            $"Payment denied, amount of [{_payment.Amount}] credit [{_credit}]");
    }    

    public override async Task<NodeExit> Execute(Context context)
    {
        _order = context.Value(_orderField);

        _credit = context.Value(_creditField);

        _payment = Payment.Create(_order.Amount);
        context.Store(_paymentField, _payment);
        
        await Task.Delay(1000);

        return _credit >= _order.Amount
            ? PreAuthorize()
            : Deny();
    }
    
    public static PreAuthorizePayment Create(CommonNamedKey nodeName)
    {
        return new PreAuthorizePayment(nodeName?.Name);
    }
}