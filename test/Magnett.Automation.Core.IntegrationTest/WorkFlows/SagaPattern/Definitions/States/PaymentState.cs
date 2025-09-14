using System.Threading.Tasks;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;

public sealed class PaymentState : Machine
{
    private PaymentState(IMachineDefinition definition, IEventBus eventBus) : base(definition, eventBus)
    {
    }

    public static async Task<PaymentState> CreateAsync(IEventBus eventBus)
    {
        var paymentState = new PaymentState(PaymentDefinition.Definition, eventBus);
        
        await paymentState.InitializeAsync();

        return paymentState;
    }
}