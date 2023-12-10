using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Entities;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Nodes;

/// <summary>
/// Confirm Order after payment, close transaction
/// </summary>
public class ConfirmOrder : NodeAsync    
{
    public ConfirmOrder(CommonNamedKey name) : base(name)
    {
    }
    
    private readonly ContextField<Order> _orderField = ContextField<Order>.Create("Order");
    
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
        var order = context.Value(_orderField);

        order.State.Dispatch(OrderStateDefinition.Action.Confirm);
        
        await Task.Delay(1000);
        
        return NodeExit.Create(
            ExitCode.Done, 
            false, 
            $"Order confirmed id [{order.Id}] [{order.State}]");
    }
}