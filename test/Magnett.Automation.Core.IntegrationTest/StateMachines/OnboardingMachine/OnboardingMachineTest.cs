using Magnett.Automation.Core.IntegrationTest.StateMachines.OnboardingMachine;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;   
using System.Threading.Tasks;
using Xunit;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.OnboardingMachine;

/// <summary>
/// Integration test for User Onboarding State Machine example.
/// This test demonstrates the complete workflow of user onboarding
/// and validates that all state transitions work correctly.
/// </summary>
public class OnboardingMachineTest
{
    [Fact]
    public void CreateDefinition_GetValidDefinition()
    {
        var definition = OnboardingStateMachineDefinition.Definition;
            
        Assert.NotNull(definition);
        Assert.NotNull(definition.InitialState);
        Assert.Equal("Registration", definition.InitialState.Key.Name);
    }

    [Fact]
    public async Task UserOnboarding_Should_Transition_Through_Complete_Workflow()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create a user onboarding
            var onboarding = new UserOnboarding("user-1", "user1@example.com", eventBus);
            Assert.Equal(OnboardingState.Registration, onboarding.CurrentState);

            // Initialize the onboarding with its state machine
            await onboarding.InitializeAsync();
            Assert.Equal(OnboardingState.Registration, onboarding.CurrentState);

            // Verify email
            var emailVerified = await onboarding.VerifyEmailAsync();
            Assert.True(emailVerified);
            Assert.Equal(OnboardingState.EmailVerification, onboarding.CurrentState);
            Assert.True(onboarding.IsEmailVerified);

            // Setup profile
            var profileSetup = await onboarding.SetupProfileAsync("John Doe", "profile.jpg");
            Assert.True(profileSetup);
            Assert.Equal(OnboardingState.ProfileSetup, onboarding.CurrentState);
            Assert.Equal("John Doe", onboarding.FullName);
            Assert.Equal("profile.jpg", onboarding.ProfilePicture);

            // Configure preferences
            var preferences = new Dictionary<string, string>
            {
                { "theme", "dark" },
                { "language", "en" },
                { "notifications", "enabled" }
            };
            var preferencesConfigured = await onboarding.ConfigurePreferencesAsync(preferences);
            Assert.True(preferencesConfigured);
            Assert.Equal(OnboardingState.Preferences, onboarding.CurrentState);
            Assert.Equal(3, onboarding.Preferences.Count);

            // Complete welcome
            var welcomeCompleted = await onboarding.CompleteWelcomeAsync();
            Assert.True(welcomeCompleted);
            Assert.Equal(OnboardingState.Welcome, onboarding.CurrentState);

            // Finish onboarding
            var onboardingFinished = await onboarding.FinishOnboardingAsync();
            Assert.True(onboardingFinished);
            Assert.Equal(OnboardingState.Completed, onboarding.CurrentState);
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task UserOnboarding_Should_Handle_Suspension_And_Resume_Workflow()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create and initialize onboarding
            var onboarding = new UserOnboarding("user-2", "user2@example.com", eventBus);
            await onboarding.InitializeAsync();

            // Verify email
            await onboarding.VerifyEmailAsync();
            Assert.Equal(OnboardingState.EmailVerification, onboarding.CurrentState);

            // Suspend onboarding
            var suspended = await onboarding.SuspendAsync("Account verification required");
            Assert.True(suspended);
            Assert.Equal(OnboardingState.Suspended, onboarding.CurrentState);
            Assert.Equal("Account verification required", onboarding.SuspensionReason);

            // Resume onboarding
            var resumed = await onboarding.ResumeAsync();
            Assert.True(resumed);
            Assert.Equal(OnboardingState.Registration, onboarding.CurrentState);
            Assert.Null(onboarding.SuspensionReason); // Suspension reason should be cleared
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task UserOnboarding_Should_Handle_Suspension_From_Multiple_States()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Test suspension from ProfileSetup state
            var onboarding1 = new UserOnboarding("user-3", "user3@example.com", eventBus);
            await onboarding1.InitializeAsync();
            await onboarding1.VerifyEmailAsync();
            await onboarding1.SetupProfileAsync("Jane Smith");
            
            var suspended1 = await onboarding1.SuspendAsync("Manual review required");
            Assert.True(suspended1);
            Assert.Equal(OnboardingState.Suspended, onboarding1.CurrentState);

            // Test suspension from Welcome state
            var onboarding2 = new UserOnboarding("user-4", "user4@example.com", eventBus);
            await onboarding2.InitializeAsync();
            await onboarding2.VerifyEmailAsync();
            await onboarding2.SetupProfileAsync("Bob Johnson");
            
            var preferences = new Dictionary<string, string> { { "theme", "light" } };
            await onboarding2.ConfigurePreferencesAsync(preferences);
            await onboarding2.CompleteWelcomeAsync();
            
            var suspended2 = await onboarding2.SuspendAsync("Security check pending");
            Assert.True(suspended2);
            Assert.Equal(OnboardingState.Suspended, onboarding2.CurrentState);
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task UserOnboarding_Should_Throw_Exception_When_Not_Initialized()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create onboarding but don't initialize
            var onboarding = new UserOnboarding("user-5", "user5@example.com", eventBus);

            // Should throw exception when trying to verify email without initialization
            await Assert.ThrowsAsync<InvalidOperationException>(() => onboarding.VerifyEmailAsync());
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task UserOnboarding_Should_Handle_Invalid_Transitions_Gracefully()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create and initialize onboarding
            var onboarding = new UserOnboarding("user-6", "user6@example.com", eventBus);
            await onboarding.InitializeAsync();

            // Try to setup profile from Registration state (should fail)
            var profileSetup = await onboarding.SetupProfileAsync("Invalid User");
            Assert.False(profileSetup); // Should return false, not throw exception
            Assert.Equal(OnboardingState.Registration, onboarding.CurrentState); // Should remain in Registration
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }
}
