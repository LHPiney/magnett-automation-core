using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.DocumentMachine;

/// <summary>
/// Document actions enumeration for the Document Processing State Machine example.
/// This demonstrates the best practice of using type-safe enumerations instead of string literals.
/// </summary>
public record DocumentAction : Enumeration
{
    public static readonly DocumentAction Submit = new(1, nameof(Submit));
    public static readonly DocumentAction Approve = new(2, nameof(Approve));
    public static readonly DocumentAction Reject = new(3, nameof(Reject));
    public static readonly DocumentAction RequestChanges = new(4, nameof(RequestChanges));
    public static readonly DocumentAction Publish = new(5, nameof(Publish));
    public static readonly DocumentAction Archive = new(6, nameof(Archive));
    public static readonly DocumentAction Revise = new(7, nameof(Revise));
    public static readonly DocumentAction Update = new(8, nameof(Update));
    public static readonly DocumentAction Restore = new(9, nameof(Restore));
    public static readonly DocumentAction Delete = new(10, nameof(Delete));

    private DocumentAction(int id, string name) : base(id, name) { }
}

