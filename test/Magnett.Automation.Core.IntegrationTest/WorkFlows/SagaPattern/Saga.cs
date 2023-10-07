using System.Threading.Tasks;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Entities;
using Magnett.Automation.Core.WorkFlows;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern;

public class Saga 
{
    private readonly ContextField<double>  _amountField       = ContextField<double>.Create("Amount");
    private readonly ContextField<string>  _descriptionField  = ContextField<string>.Create("Description");
    private readonly ContextField<double>  _creditLimitField  = ContextField<double>.Create("CreditLimit");
    private readonly ContextField<bool>    _canMakePayment    = ContextField<bool>.Create("CanMakePayment");
    private readonly ContextField<Order>   _orderField        = ContextField<Order>.Create("Order");
    private readonly ContextField<Payment> _paymentField      = ContextField<Payment>.Create("Payment");
    
    private readonly IFlow _flow;
    
    private Saga(SagaRequest request)
    {
        var (amount, description, creditLimit, canMakePayment) = request;

        var context = Context.Create()
            .Store(_amountField, amount)
            .Store(_descriptionField, description)
            .Store(_creditLimitField, creditLimit)
            .Store(_canMakePayment, canMakePayment);

        _flow = Flow.Create(
            FlowRunner.Create(SagaPatternDefinition.Definition,
            context));
    }

    public async Task<SagaResponse> Execute()
    {
      var flowExit = await _flow.Run();

      return new SagaResponse(
          _flow.Context.Value(_orderField),
          _flow.Context.Value(_paymentField));
    }

    public static Saga Create(SagaRequest request)
    {
        return new Saga(request);
    }
}