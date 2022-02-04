using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;

public class PaymentState : Machine
{
    private PaymentState(IMachineDefinition definition) : base(definition)
    {
    }

    public static PaymentState Create()
    {
        return new PaymentState(PaymentStateDefinition.GetDefinition());
    }
}