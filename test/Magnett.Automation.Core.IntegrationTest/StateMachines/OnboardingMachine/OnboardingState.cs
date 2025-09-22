using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.OnboardingMachine;

/// <summary>
/// User onboarding states enumeration for the User Onboarding State Machine example.
/// This demonstrates the best practice of using type-safe enumerations instead of string literals.
/// </summary>
public record OnboardingState : Enumeration
{
    public static readonly OnboardingState Registration = new(1, nameof(Registration));
    public static readonly OnboardingState EmailVerification = new(2, nameof(EmailVerification));
    public static readonly OnboardingState ProfileSetup = new(3, nameof(ProfileSetup));
    public static readonly OnboardingState Preferences = new(4, nameof(Preferences));
    public static readonly OnboardingState Welcome = new(5, nameof(Welcome));
    public static readonly OnboardingState Completed = new(6, nameof(Completed));
    public static readonly OnboardingState Suspended = new(7, nameof(Suspended));

    private OnboardingState(int id, string name) : base(id, name) { }
}
