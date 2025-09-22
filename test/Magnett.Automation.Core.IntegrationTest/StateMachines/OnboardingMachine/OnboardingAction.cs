using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.OnboardingMachine;

/// <summary>
/// User onboarding actions enumeration for the User Onboarding State Machine example.
/// This demonstrates the best practice of using type-safe enumerations instead of string literals.
/// </summary>
public record OnboardingAction : Enumeration
{
    public static readonly OnboardingAction VerifyEmail = new(1, nameof(VerifyEmail));
    public static readonly OnboardingAction SetupProfile = new(2, nameof(SetupProfile));
    public static readonly OnboardingAction ConfigurePreferences = new(3, nameof(ConfigurePreferences));
    public static readonly OnboardingAction CompleteWelcome = new(4, nameof(CompleteWelcome));
    public static readonly OnboardingAction Finish = new(5, nameof(Finish));
    public static readonly OnboardingAction Suspend = new(6, nameof(Suspend));
    public static readonly OnboardingAction Resume = new(7, nameof(Resume));

    private OnboardingAction(int id, string name) : base(id, name) { }
}
