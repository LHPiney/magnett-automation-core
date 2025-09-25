using System;
using System.Threading.Tasks;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Entities;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern;

public class Saga 
{
    private readonly SagaRequest _request;
    private readonly ILogger<Saga> _logger;
    private readonly IEventBus _eventBus;
    
    private readonly ContextField<double>  _amountField       = ContextField<double>.WithName("Amount");
    private readonly ContextField<string>  _descriptionField  = ContextField<string>.WithName("Description");
    private readonly ContextField<double>  _creditLimitField  = ContextField<double>.WithName("CreditLimit");
    private readonly ContextField<bool>    _canMakePayment    = ContextField<bool>.WithName("CanMakePayment");
    private readonly ContextField<Order>   _orderField        = ContextField<Order>.WithName("Order");
    private readonly ContextField<Payment> _paymentField      = ContextField<Payment>.WithName("Payment");
    
    private Saga(SagaRequest request)
    {
        _request = request ?? throw new ArgumentNullException(nameof(request));
        var factory = LoggerFactory.Create(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Trace);
        });

        _logger = factory.CreateLogger<Saga>();
        _eventBus = EventBus.Create(factory.CreateLogger<EventBus>());
    }

    private async Task<Context> InitializeContext()
    {
        var context = Context.Create(_eventBus);
        
        var tasks = new Task[]
        {
            context.StoreAsync(_amountField, _request.Amount),
            context.StoreAsync(_descriptionField, _request.Description),
            context.StoreAsync(_creditLimitField, _request.CreditLimit),
            context.StoreAsync(_canMakePayment, _request.CanMakePayment)
        };
        
        await Task.WhenAll(tasks);
        
        return context;
    }

    public async Task<SagaResponse> Execute()
    {
        var context    = await InitializeContext();
        var flowRunner = FlowRunner.Create(SagaPatternDefinition.Definition, context, _eventBus);
        var flow       = Flow.Create(flowRunner);
        
        _logger.LogTrace("Saga started with amount: {Amount}, description: {Description}, credit limit: {CreditLimit}, can make payment: {CanMakePayment}",
            context.Value(_amountField),
            context.Value(_descriptionField),
            context.Value(_creditLimitField),
            context.Value(_canMakePayment));
        
        var flowExit= flow.Run();
        
        await Task.WhenAll(flowExit);
        
        return new SagaResponse(
            context.Value(_orderField),
            context.Value(_paymentField),
            flowExit.Result.State == ExitState.Failed);
    }

    public static Saga Create(SagaRequest request)
    {
        return new Saga(request);
    }
}