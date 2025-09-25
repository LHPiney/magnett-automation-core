using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.DocumentMachine;

/// <summary>
/// Document states enumeration for the Document Processing State Machine example.
/// This demonstrates the best practice of using type-safe enumerations instead of string literals.
/// </summary>
public record DocumentState : Enumeration
{
    public static readonly DocumentState Draft = new(1, nameof(Draft));
    public static readonly DocumentState UnderReview = new(2, nameof(UnderReview));
    public static readonly DocumentState Approved = new(3, nameof(Approved));
    public static readonly DocumentState Rejected = new(4, nameof(Rejected));
    public static readonly DocumentState Published = new(5, nameof(Published));
    public static readonly DocumentState Archived = new(6, nameof(Archived));
    public static readonly DocumentState Deleted = new(7, nameof(Deleted));

    private DocumentState(int id, string name) : base(id, name) { }
}

