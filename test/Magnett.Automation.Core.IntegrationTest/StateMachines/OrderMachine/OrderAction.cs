using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.OrderMachine;

/// <summary>
/// Order actions enumeration for the Order Management State Machine example.
/// This demonstrates the best practice of using type-safe enumerations instead of string literals.
/// </summary>
public record OrderAction : Enumeration
{
    public static readonly OrderAction Confirm = new(1, nameof(Confirm));
    public static readonly OrderAction StartProcessing = new(2, nameof(StartProcessing));
    public static readonly OrderAction Ship = new(3, nameof(Ship));
    public static readonly OrderAction Deliver = new(4, nameof(Deliver));
    public static readonly OrderAction Cancel = new(5, nameof(Cancel));
    public static readonly OrderAction Return = new(6, nameof(Return));

    private OrderAction(int id, string name) : base(id, name) { }
}
