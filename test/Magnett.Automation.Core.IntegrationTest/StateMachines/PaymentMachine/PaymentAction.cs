using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.PaymentMachine;

/// <summary>
/// Payment actions enumeration for the Payment Processing State Machine example.
/// This demonstrates the best practice of using type-safe enumerations instead of string literals.
/// </summary>
public record PaymentAction : Enumeration
{
    public static readonly PaymentAction Process = new(1, nameof(Process));
    public static readonly PaymentAction Complete = new(2, nameof(Complete));
    public static readonly PaymentAction Fail = new(3, nameof(Fail));
    public static readonly PaymentAction Retry = new(4, nameof(Retry));
    public static readonly PaymentAction Cancel = new(5, nameof(Cancel));
    public static readonly PaymentAction Refund = new(6, nameof(Refund));

    private PaymentAction(int id, string name) : base(id, name) { }
}
