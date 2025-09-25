#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magnett.Automation.Core.Events;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.OnboardingMachine;

/// <summary>
/// User onboarding entity that encapsulates a state machine for managing user onboarding lifecycle.
/// This demonstrates how to embed a state machine within a domain entity and expose
/// business methods that dispatch actions on the internal state machine.
/// </summary>
public class UserOnboarding
{
    private readonly IEventBus _eventBus;
    private OnboardingStateMachine? _machine;
    
    public string UserId { get; }
    public string Email { get; }
    public OnboardingState CurrentState => _machine?.CurrentOnboardingState ?? OnboardingState.Registration;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public string? FullName { get; private set; }
    public string? ProfilePicture { get; private set; }
    public Dictionary<string, string> Preferences { get; }
    public string? SuspensionReason { get; private set; }
    public bool IsEmailVerified { get; private set; }

    public UserOnboarding(string userId, string email, IEventBus eventBus)
    {
        UserId = userId;
        Email = email;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Preferences = new Dictionary<string, string>();
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public async Task InitializeAsync()
    {
        _machine = await OnboardingStateMachine.CreateAsync(_eventBus);
        UpdatedAt = DateTime.UtcNow;
    }

    public async Task<bool> VerifyEmailAsync()
    {
        if (_machine == null) throw new InvalidOperationException("User onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.VerifyEmail);
            IsEmailVerified = true;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> SetupProfileAsync(string fullName, string? profilePicture = null)
    {
        if (_machine == null) throw new InvalidOperationException("User onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.SetupProfile);
            FullName = fullName;
            ProfilePicture = profilePicture;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ConfigurePreferencesAsync(Dictionary<string, string> preferences)
    {
        if (_machine == null) throw new InvalidOperationException("User onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.ConfigurePreferences);
            foreach (var preference in preferences)
            {
                Preferences[preference.Key] = preference.Value;
            }
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CompleteWelcomeAsync()
    {
        if (_machine == null) throw new InvalidOperationException("User onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.CompleteWelcome);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> FinishOnboardingAsync()
    {
        if (_machine == null) throw new InvalidOperationException("User onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.Finish);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> SuspendAsync(string reason)
    {
        if (_machine == null) throw new InvalidOperationException("User onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.Suspend);
            SuspensionReason = reason;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ResumeAsync()
    {
        if (_machine == null) throw new InvalidOperationException("User onboarding not initialized");
        
        try
        {
            await _machine.DispatchAsync(OnboardingAction.Resume);
            SuspensionReason = null; // Clear suspension reason
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
