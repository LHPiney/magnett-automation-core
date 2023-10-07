using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Entities;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;
using Magnett.Automation.Core.WorkFlows;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Nodes;

/// <summary>
/// Cancel payment. and rollback pre-authorization
/// </summary>
public class CancelPayment : NodeAsync    
{
    private CancelPayment(string name) : base(name)
    {
    }
    
    private readonly ContextField<Payment> _paymentField = ContextField<Payment>.Create("Payment");
    
    #region ExitCodes

    public class ExitCode : Enumeration
    {
        public static readonly ExitCode Done = new ExitCode(1, nameof(Done)); 

        private ExitCode(int id, string name) : base(id, name)
        {
        }
    }
        
    #endregion

    public override async Task<NodeExit> Execute(Context context)
    {
        var payment = context.Value(_paymentField);

        payment.State.Dispatch(PaymentStateDefinition.Action.Cancel);
        
        await Task.Delay(1000);
        
        return NodeExit.Create(
            ExitCode.Done, 
            true, 
            $"Payment cancelled id [{payment.Id}] [{payment.State}]");
    }
    
    public static CancelPayment Create(CommonNamedKey nodeName)
    {
        return new CancelPayment(nodeName?.Name);
    }
}