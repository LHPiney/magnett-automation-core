using System.Threading.Tasks;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.PaymentMachine;

/// <summary>
/// Payment state machine implementation that inherits from Machine.
/// This demonstrates the recommended pattern for creating state machines.
/// </summary>
public sealed class PaymentStateMachine : Machine
{
    private PaymentStateMachine(IMachineDefinition definition, IEventBus eventBus) : base(definition, eventBus)
    {
        
    }

    public static async Task<PaymentStateMachine> CreateAsync(IEventBus eventBus)
    {
        var machine = new PaymentStateMachine(PaymentStateMachineDefinition.Definition, eventBus);
        await machine.InitializeAsync();
        return machine;
    }

    /// <summary>
    /// Gets the current state as a PaymentState enumeration.
    /// This provides a type-safe way to access the current state.
    /// </summary>
    public PaymentState CurrentPaymentState => this switch
    {
        var machine when machine.Equals(PaymentState.Pending) => PaymentState.Pending,
        var machine when machine.Equals(PaymentState.Processing) => PaymentState.Processing,
        var machine when machine.Equals(PaymentState.Completed) => PaymentState.Completed,
        var machine when machine.Equals(PaymentState.Failed) => PaymentState.Failed,
        var machine when machine.Equals(PaymentState.Cancelled) => PaymentState.Cancelled,
        var machine when machine.Equals(PaymentState.Refunded) => PaymentState.Refunded,
        _ => PaymentState.Pending
    };
}
