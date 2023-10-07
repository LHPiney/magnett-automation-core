using System;
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
/// Create order record, begin transaction
/// </summary>
public class CreateOrder : NodeAsync    
{
    private readonly ContextField<Order> _orderField = ContextField<Order>.Create("Order");
    private readonly ContextField<double> _amountField = ContextField<double>.Create("Amount");
    private readonly ContextField<string> _descriptionField = ContextField<string>.Create("Description");
    
    #region ExitCodes

    public class ExitCode : Enumeration
    {
        public static readonly ExitCode Created  = new ExitCode(1, nameof(Created)); 

        private ExitCode(int id, string name) : base(id, name)
        {
        }
    }
        
    #endregion
    
    private CreateOrder(string name) : base(name)
    {
    }

    public override async Task<NodeExit> Execute(Context context)
    {
        var order = Order.Create(
            context.Value(_amountField), 
            context.Value(_descriptionField));

        order.State.Dispatch(OrderStateDefinition.Action.Validate);
        
        context.Store(_orderField, order);
        
        await Task.Delay(1000);
        
        return NodeExit.Create(
            ExitCode.Created, 
            false, 
            $"Order id [{order.Id}] [{order.State.State.Key}]");
    }

    public static CreateOrder Create(CommonNamedKey nodeName)
    {
        return new CreateOrder(nodeName?.Name);
    }
}