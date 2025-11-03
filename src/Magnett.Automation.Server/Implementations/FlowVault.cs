using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;
using Magnett.Automation.Server.Events;
using Magnett.Automation.Server.Repositories;
using Magnett.Automation.Server.Repositories.Implementations;
using Microsoft.Extensions.Logging;


namespace Magnett.Automation.Server.Implementations;

/// <summary>
/// Vault for managing workflow definitions and instances.
/// Inherits from EventEmitterEntity to provide event emission capabilities.
/// Use EmitEventAsync&lt;TEvent&gt; to emit events for state changes and operations.
/// </summary>
public class FlowVault(
    IFlowDefinitionRegistry definitionRegistry,
    IFlowRepository repository,
    IEventBus eventBus,
    ILogger<FlowVault> logger) : EventEmitterEntity(eventBus), IFlowVault
{
    private readonly IFlowDefinitionRegistry _definitionRegistry = definitionRegistry ?? throw new ArgumentNullException(nameof(definitionRegistry));
    private readonly IFlowRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly ILogger<FlowVault> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Result<IFlowDefinition>> GetDefinitionAsync(string key, int? version, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result<IFlowDefinition>.Failure("Key cannot be null or empty");
        }

        var result = await _definitionRegistry.GetAsync(key, version, ct);
        
        if (result.IsSuccess)
        {
            await EmitEventAsync(
                FlowDefinitionRetrievedEvent.Create(
                    key,
                    version,
                    DateTime.UtcNow),
                ct);
        }

        return result;
    }

    public async Task<Result<int>> PublishDefinitionAsync(IFlowDefinition definition, CancellationToken ct)
    {
        if (definition == null)
        {
            return Result<int>.Failure("Definition cannot be null");
        }

        var registerResult = await _definitionRegistry.RegisterAsync(definition, ct);
        if (registerResult.IsFailure)
        {
            await EmitEventAsync(
                FlowDefinitionPublishFailedEvent.Create(
                    definition.Key.Name,
                    definition.Version,
                    registerResult.ErrorMessage ?? "Failed to publish definition",
                    registerResult.Exception),
                ct);
            
            return Result<int>.Failure(registerResult.ErrorMessage ?? "Failed to publish definition", registerResult.Exception);
        }

        await EmitEventAsync(
            FlowDefinitionPublishedEvent.Create(
                definition.Key.Name,
                definition.Version,
                DateTime.UtcNow),
            ct);

        return Result<int>.Success(definition.Version);
    }

    public async Task<Result<IFlow>> CreateInstanceAsync(string definitionKey, Context initialContext, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(definitionKey))
        {
            return Result<IFlow>.Failure("Definition key cannot be null or empty");
        }

        if (initialContext == null)
        {
            return Result<IFlow>.Failure("Initial context cannot be null");
        }

        var definitionResult = await _definitionRegistry.GetAsync(definitionKey, null, ct);
        if (definitionResult.IsFailure)
        {
            await EmitEventAsync(
                FlowInstanceCreationFailedEvent.Create(
                    definitionKey,
                    definitionResult.ErrorMessage ?? "Failed to get definition",
                    definitionResult.Exception),
                ct);
            
            return Result<IFlow>.Failure(definitionResult.ErrorMessage ?? "Failed to get definition", definitionResult.Exception);
        }

        var definition = definitionResult.Value!;
        
        var flowRunnerLogger = _logger is ILogger<FlowRunner> flowRunnerTypedLogger
            ? flowRunnerTypedLogger
            : Microsoft.Extensions.Logging.LoggerFactory
                .Create(builder => builder.AddConsole())
                .CreateLogger<FlowRunner>();
        
        var flowRunner = FlowRunner.Create(definition, initialContext, EventBus!, flowRunnerLogger);
        var flow = Flow.Create(flowRunner);

        var saveResult = await _repository.SaveAsync(flow, FlowState.Running, ct);
        if (saveResult.IsFailure)
        {
            await EmitEventAsync(
                FlowInstanceCreationFailedEvent.Create(
                    definitionKey,
                    saveResult.ErrorMessage ?? "Failed to save flow instance",
                    saveResult.Exception),
                ct);
            
            return Result<IFlow>.Failure(saveResult.ErrorMessage ?? "Failed to save flow instance", saveResult.Exception);
        }

        await EmitEventAsync(
            FlowInstanceCreatedEvent.Create(
                flow.Id,
                definitionKey,
                definition.Version,
                DateTime.UtcNow),
            ct);

        _logger.LogInformation("Created workflow instance {FlowId} from definition {DefinitionKey}", flow.Id, definitionKey);

        return Result<IFlow>.Success(flow);
    }

    public async Task<Result<IFlow>> GetInstanceAsync(Guid id, CancellationToken ct)
    {
        var result = await _repository.GetAsync(id, ct);
        
        if (result.IsSuccess)
        {
            await EmitEventAsync(
                FlowInstanceRetrievedEvent.Create(
                    id,
                    DateTime.UtcNow),
                ct);
        }

        return result;
    }

    public async Task<Result<IEnumerable<IFlow>>> GetRunningInstancesAsync(CancellationToken ct)
    {
        return await _repository.GetByStateAsync(FlowState.Running, ct);
    }

    public async Task<Result> SaveInstanceAsync(IFlow flow, CancellationToken ct)
    {
        if (flow == null)
        {
            return Result.Failure("Flow cannot be null");
        }

        var existsResult = await _repository.ExistsAsync(flow.Id, ct);
        if (existsResult.IsFailure)
        {
            return Result.Failure(existsResult.ErrorMessage ?? "Failed to check if flow exists", existsResult.Exception);
        }

        if (!existsResult.Value)
        {
            return Result.Failure($"Flow instance with id '{flow.Id}' not found");
        }

        var stateResult = await _repository.GetStateAsync(flow.Id, ct);
        if (stateResult.IsFailure)
        {
            await EmitEventAsync(
                FlowInstanceSaveFailedEvent.Create(
                    flow.Id,
                    stateResult.ErrorMessage ?? "Failed to get flow state",
                    stateResult.Exception),
                ct);
            
            return Result.Failure(stateResult.ErrorMessage ?? "Failed to get flow state", stateResult.Exception);
        }

        var saveResult = await _repository.SaveAsync(flow, stateResult.Value, ct);
        if (saveResult.IsFailure)
        {
            await EmitEventAsync(
                FlowInstanceSaveFailedEvent.Create(
                    flow.Id,
                    saveResult.ErrorMessage ?? "Failed to save flow instance",
                    saveResult.Exception),
                ct);
            
            return Result.Failure(saveResult.ErrorMessage ?? "Failed to save flow instance", saveResult.Exception);
        }

        await EmitEventAsync(
            FlowInstanceSavedEvent.Create(
                flow.Id,
                stateResult.Value.Name,
                DateTime.UtcNow),
            ct);

        _logger.LogDebug("Saved workflow instance {FlowId}", flow.Id);

        return Result.Success();
    }

    public async Task<Result> PauseInstanceAsync(Guid id, CancellationToken ct)
    {
        var existsResult = await _repository.ExistsAsync(id, ct);
        if (existsResult.IsFailure || !existsResult.Value)
        {
            return Result.Failure($"Flow instance with id '{id}' not found");
        }

        var stateResult = await _repository.GetStateAsync(id, ct);
        if (stateResult.IsFailure)
        {
            return Result.Failure(stateResult.ErrorMessage ?? "Failed to get flow state", stateResult.Exception);
        }

        if (stateResult.Value == FlowState.Paused)
        {
            return Result.Success();
        }

        var previousStateName = stateResult.Value.Name;

        var updateResult = await _repository.UpdateStateAsync(id, FlowState.Paused, ct);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.ErrorMessage ?? "Failed to pause flow instance", updateResult.Exception);
        }

        await EmitEventAsync(
            FlowInstancePausedEvent.Create(
                id,
                DateTime.UtcNow),
            ct);

        await EmitEventAsync(
            FlowInstanceStateChangedEvent.Create(
                id,
                previousStateName,
                FlowState.Paused.Name,
                DateTime.UtcNow),
            ct);

        _logger.LogInformation("Paused workflow instance {FlowId}", id);

        return Result.Success();
    }

    public async Task<Result> ResumeInstanceAsync(Guid id, CancellationToken ct)
    {
        var existsResult = await _repository.ExistsAsync(id, ct);
        if (existsResult.IsFailure || !existsResult.Value)
        {
            return Result.Failure($"Flow instance with id '{id}' not found");
        }

        var stateResult = await _repository.GetStateAsync(id, ct);
        if (stateResult.IsFailure)
        {
            return Result.Failure(stateResult.ErrorMessage ?? "Failed to get flow state", stateResult.Exception);
        }

        var currentState = stateResult.Value;
        if (currentState != FlowState.Paused)
        {
            return Result.Failure($"Flow instance with id '{id}' is not paused. Current state: {currentState.Name}");
        }

        var previousStateName = FlowState.Paused.Name;

        var updateResult = await _repository.UpdateStateAsync(id, FlowState.Running, ct);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.ErrorMessage ?? "Failed to resume flow instance", updateResult.Exception);
        }

        await EmitEventAsync(
            FlowInstanceResumedEvent.Create(
                id,
                DateTime.UtcNow),
            ct);

        await EmitEventAsync(
            FlowInstanceStateChangedEvent.Create(
                id,
                previousStateName,
                FlowState.Running.Name,
                DateTime.UtcNow),
            ct);

        _logger.LogInformation("Resumed workflow instance {FlowId}", id);

        return Result.Success();
    }

    public static FlowVault Create(
        IFlowDefinitionRegistry definitionRegistry,
        IFlowRepository repository,
        IEventBus eventBus,
        ILogger<FlowVault> logger)
    {
        return new FlowVault(definitionRegistry, repository, eventBus, logger);
    }

    public static FlowVault Create(
        IFlowDefinitionRegistry definitionRegistry,
        IEventBus eventBus,
        ILogger<FlowVault> logger)
    {
        var repository = InMemoryFlowRepository.Create();
        return new FlowVault(definitionRegistry, repository, eventBus, logger);
    }
}

