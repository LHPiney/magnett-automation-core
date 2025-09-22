using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.PaymentMachine;

/// <summary>
/// Payment states enumeration for the Payment Processing State Machine example.
/// This demonstrates the best practice of using type-safe enumerations instead of string literals.
/// </summary>
public record PaymentState : Enumeration
{
    public static readonly PaymentState Pending = new(1, nameof(Pending));
    public static readonly PaymentState Processing = new(2, nameof(Processing));
    public static readonly PaymentState Completed = new(3, nameof(Completed));
    public static readonly PaymentState Failed = new(4, nameof(Failed));
    public static readonly PaymentState Cancelled = new(5, nameof(Cancelled));
    public static readonly PaymentState Refunded = new(6, nameof(Refunded));

    private PaymentState(int id, string name) : base(id, name) { }
}
