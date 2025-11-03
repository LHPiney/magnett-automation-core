using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Runtimes;

namespace Magnett.Automation.Server;

/// <summary>
/// Vault interface for managing workflow definitions and instances.
/// This is the core interface for server-side workflow orchestration,
/// enabling persistence, recovery, and multi-instance management.
/// Designed as the foundation for SaaS workflow platforms.
/// </summary>
public interface IFlowVault
{
    /// <summary>
    /// Gets a workflow definition by key and optional version (null = latest).
    /// </summary>
    Task<Result<IFlowDefinition>> GetDefinitionAsync(string key, int? version, CancellationToken ct);
    
    /// <summary>
    /// Publishes a workflow definition and returns its effective version.
    /// This may increment the version if the definition already exists.
    /// </summary>
    Task<Result<int>> PublishDefinitionAsync(IFlowDefinition definition, CancellationToken ct);
    
    /// <summary>
    /// Creates a new workflow instance from a published definition.
    /// The instance is persisted and can be recovered later.
    /// </summary>
    Task<Result<IFlow>> CreateInstanceAsync(string definitionKey, Context initialContext, CancellationToken ct);
    
    /// <summary>
    /// Gets a workflow instance by its unique identifier.
    /// Returns null if the instance does not exist.
    /// </summary>
    Task<Result<IFlow>> GetInstanceAsync(Guid id, CancellationToken ct);
    
    /// <summary>
    /// Gets all currently running workflow instances.
    /// Useful for monitoring and management operations.
    /// </summary>
    Task<Result<IEnumerable<IFlow>>> GetRunningInstancesAsync(CancellationToken ct);
    
    /// <summary>
    /// Saves the current state of a workflow instance.
    /// Enables persistence and recovery of workflow execution state.
    /// </summary>
    Task<Result> SaveInstanceAsync(IFlow flow, CancellationToken ct);
    
    /// <summary>
    /// Pauses a running workflow instance.
    /// The instance state is saved and can be resumed later.
    /// </summary>
    Task<Result> PauseInstanceAsync(Guid id, CancellationToken ct);
    
    /// <summary>
    /// Resumes a paused workflow instance.
    /// The instance state is restored and execution continues.
    /// </summary>
    Task<Result> ResumeInstanceAsync(Guid id, CancellationToken ct);
}

