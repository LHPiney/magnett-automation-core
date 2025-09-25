using System.Threading.Tasks;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.OnboardingMachine;

/// <summary>
/// User onboarding state machine implementation that inherits from Machine.
/// This demonstrates the recommended pattern for creating state machines.
/// </summary>
public sealed class OnboardingStateMachine : Machine
{
    private OnboardingStateMachine(IMachineDefinition definition, IEventBus eventBus) : base(definition, eventBus)
    {
        
    }

    public static async Task<OnboardingStateMachine> CreateAsync(IEventBus eventBus)
    {
        var machine = new OnboardingStateMachine(OnboardingStateMachineDefinition.Definition, eventBus);
        await machine.InitializeAsync();
        return machine;
    }

    /// <summary>
    /// Gets the current state as an OnboardingState enumeration.
    /// This provides a type-safe way to access the current state.
    /// </summary>
    public OnboardingState CurrentOnboardingState => this switch
    {
        var machine when machine.Equals(OnboardingState.Registration) => OnboardingState.Registration,
        var machine when machine.Equals(OnboardingState.EmailVerification) => OnboardingState.EmailVerification,
        var machine when machine.Equals(OnboardingState.ProfileSetup) => OnboardingState.ProfileSetup,
        var machine when machine.Equals(OnboardingState.Preferences) => OnboardingState.Preferences,
        var machine when machine.Equals(OnboardingState.Welcome) => OnboardingState.Welcome,
        var machine when machine.Equals(OnboardingState.Completed) => OnboardingState.Completed,
        var machine when machine.Equals(OnboardingState.Suspended) => OnboardingState.Suspended,
        _ => OnboardingState.Registration
    };
}
