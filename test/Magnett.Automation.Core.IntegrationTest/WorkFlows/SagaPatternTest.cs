using System.Threading.Tasks;
using Xunit;

using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern;
using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows;

public class SagaPatternTest
{
    [Fact]
    public async Task SagaPattern_WhenInvokeWithCredit_ShouldReturnOrderConfirmedPaymentConfirmed()
    {
        var saga = Saga.Create(
            new SagaRequest(
                10.0,
                "Operation with credit", 
                20.0, 
                true));

        var result = await saga.Execute();

        Assert.NotNull(result);
        Assert.True(result.Order.State.Equals(OrderDefinition.Status.Confirmed));
        Assert.True(result.Payment.State.Equals(PaymentDefinition.Status.Confirmed));
    }
    
    [Fact]
    public async Task SagaPattern_WhenInvokeWithOutCredit_ShouldReturnOrderCancelledPaymentCancelled()
    {
        var saga = Saga.Create(
            new SagaRequest(
                50.0, 
                "Operation without credit", 
                20.0, 
                true));

        var result = await saga.Execute();
        
        Assert.NotNull(result);
        Assert.True(result.Order.State.Equals(OrderDefinition.Status.Cancelled));
        Assert.True(result.Payment.State.Equals(PaymentDefinition.Status.Cancelled));
    }
}   