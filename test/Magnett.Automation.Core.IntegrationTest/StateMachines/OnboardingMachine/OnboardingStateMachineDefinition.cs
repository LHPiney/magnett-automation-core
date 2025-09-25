using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Builders;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.OnboardingMachine;

/// <summary>
/// State machine definition for user onboarding workflow.
/// This demonstrates how to define a complete state machine with all possible transitions.
/// </summary>
public static class OnboardingStateMachineDefinition
{
    public static IMachineDefinition Definition { get; } = MachineDefinitionBuilder.Create()
        .InitialState(OnboardingState.Registration)
            .OnAction(OnboardingAction.VerifyEmail).ToState(OnboardingState.EmailVerification)
            .OnAction(OnboardingAction.Suspend).ToState(OnboardingState.Suspended)
            .Build()
        .AddState(OnboardingState.EmailVerification)
            .OnAction(OnboardingAction.SetupProfile).ToState(OnboardingState.ProfileSetup)
            .OnAction(OnboardingAction.Suspend).ToState(OnboardingState.Suspended)
            .Build()
        .AddState(OnboardingState.ProfileSetup)
            .OnAction(OnboardingAction.ConfigurePreferences).ToState(OnboardingState.Preferences)
            .OnAction(OnboardingAction.Suspend).ToState(OnboardingState.Suspended)
            .Build()
        .AddState(OnboardingState.Preferences)
            .OnAction(OnboardingAction.CompleteWelcome).ToState(OnboardingState.Welcome)
            .OnAction(OnboardingAction.Suspend).ToState(OnboardingState.Suspended)
            .Build()
        .AddState(OnboardingState.Welcome)
            .OnAction(OnboardingAction.Finish).ToState(OnboardingState.Completed)
            .OnAction(OnboardingAction.Suspend).ToState(OnboardingState.Suspended)
            .Build()
        .AddState(OnboardingState.Completed)
            .Build()
        .AddState(OnboardingState.Suspended)
            .OnAction(OnboardingAction.Resume).ToState(OnboardingState.Registration)
            .Build()
        .BuildDefinition();
}
