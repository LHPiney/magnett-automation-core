using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.OrderMachine;

/// <summary>
/// Order states enumeration for the Order Management State Machine example.
/// This demonstrates the best practice of using type-safe enumerations instead of string literals.
/// </summary>
public record OrderState : Enumeration
{
    public static readonly OrderState Pending = new(1, nameof(Pending));
    public static readonly OrderState Confirmed = new(2, nameof(Confirmed));
    public static readonly OrderState Processing = new(3, nameof(Processing));
    public static readonly OrderState Shipped = new(4, nameof(Shipped));
    public static readonly OrderState Delivered = new(5, nameof(Delivered));
    public static readonly OrderState Cancelled = new(6, nameof(Cancelled));
    public static readonly OrderState Returned = new(7, nameof(Returned));

    private OrderState(int id, string name) : base(id, name) { }
}
