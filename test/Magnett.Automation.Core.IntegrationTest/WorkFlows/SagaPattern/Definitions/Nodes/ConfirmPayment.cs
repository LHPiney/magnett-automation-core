using System.Threading.Tasks;

using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Entities;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Nodes;

/// <summary>
/// Try to make payment 
/// </summary>
public class ConfirmPayment : NodeAsync    
{
    private readonly ContextField<bool> _canMakePayment = ContextField<bool>.Create("CanMakePayment");
    private readonly ContextField<Payment> _paymentField = ContextField<Payment>.Create("Payment");

    public ConfirmPayment(CommonNamedKey name) : base(name)
    {
    }
    
    #region ExitCodes

    public class ExitCode : Enumeration
    {
        public static readonly ExitCode Done = new ExitCode(1, nameof(Done)); 
        public static readonly ExitCode Failed = new ExitCode(1, nameof(Failed)); 
        
        private ExitCode(int id, string name) : base(id, name)
        {
        }
    }
        
    #endregion

    private NodeExit Confirm(Payment payment)
    {
        payment.State.Dispatch(PaymentStateDefinition.Action.Confirm);

        return NodeExit.Create(
            ExitCode.Done,
            false,
            $"Payment is Done");
    }
    
    private NodeExit Fail(Payment payment)
    {
        payment.State.Dispatch(PaymentStateDefinition.Action.Cancel);

        return NodeExit.Create(
            ExitCode.Failed,
            true,
            $"Payment Failed");
    }


    public override async Task<NodeExit> Execute(Context context)
    {
        var payment = context.Value(_paymentField);

        await Task.Delay(1000);

        return context.Value(_canMakePayment)
            ? Confirm(payment)
            : Fail(payment);
    }
}