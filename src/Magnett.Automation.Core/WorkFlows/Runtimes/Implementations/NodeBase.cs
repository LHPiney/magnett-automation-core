#nullable enable
using System.Threading;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Builders;
using Magnett.Automation.Core.WorkFlows.Runtimes.Events;

namespace Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

public abstract class NodeBase(CommonNamedKey key, IEventBus eventBus): EventEmitterEntity(eventBus)
{
    protected IState? CurrentState => States?.Current; 
    protected IMachine? States { get; private set; }
    public NodeState? State =>  NodeState.WithName(CurrentState?.Key);
    public CommonNamedKey Key { get; } = key ?? throw new ArgumentNullException(nameof(key));
    public bool IsInitialized => States is not null && CurrentState is not null;

    protected NodeBase(string name, IEventBus eventBus) : this(CommonNamedKey.Create(name), eventBus)
    {
            
    }

    public async Task Init(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) return;
        await Initialize(cancellationToken);

        await TransitStateAsync(NodeAction.Init, OnNodeInitEvent.Create(Key), cancellationToken);
    }

    protected virtual async Task Initialize(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) return;
        States = await Machine.CreateAsync(InitStatesDefinition, EventBus);
        await States.ReStartAsync();
    }

    protected virtual async Task<NodeExit> ManageResponse(NodeExit response, CancellationToken cancellationToken = default)
    {
        return response switch
        {
            null => NodeExit.Failed("Unknown", "UNDEFINED_NODE_RESPONSE"),
            not null when response.State == ExitState.Completed => await ProcessCompletedNode(),
            not null when response.State == ExitState.Failed => await ProcessFailedNode(),
            not null when response.State == ExitState.Paused => await ProcessPausedNode(),
            not null when response.State == ExitState.Cancelled => await ProcessCancelledNode(),
            _ => throw new ArgumentOutOfRangeException(nameof(response), response, null)
        };
        
        async Task<NodeExit> ProcessNodeStatus<TEvent>(NodeAction action, TEvent @event)
            where TEvent : IEvent
        {
            await TransitStateAsync(action, @event, cancellationToken);

            return response;
        }

        async Task<NodeExit> ProcessCompletedNode()
        {
            return await ProcessNodeStatus(
                NodeAction.Complete,
                OnNodeCompletedEvent.Create(Key, response.Code, response.Data));
        }

        async Task<NodeExit> ProcessFailedNode()
        {
            return await ProcessNodeStatus(
                NodeAction.Fail,
                OnNodeFailedEvent.Create(Key, response.Code, response.Data));
        }

        async Task<NodeExit> ProcessPausedNode()
        {
            return await ProcessNodeStatus(
                NodeAction.Pause,
                OnNodePausedEvent.Create(Key, response.Code, response.Data));
        }

        async Task<NodeExit> ProcessCancelledNode()
        {
            return await ProcessNodeStatus(
                NodeAction.Cancel,
                OnNodeCancelledEvent.Create(Key, response.Code, response.Data));
        }
    }

    /// <summary>
    /// Transits the current state of the state machine using the specified state transition action
    /// and publishes the given event to the event bus.
    /// </summary>
    /// <param name="stateTransition">
    /// The state transition action to be applied on the state machine.
    /// </param>
    /// <param name="eventToPublish">
    /// The event to be published after the state transition is successfully completed.
    /// </param>
    /// <typeparam name="TEvent">
    /// The type of the event to be published, which must implement the <see cref="IEvent"/> interface.
    /// </typeparam>
    protected async Task TransitStateAsync<TEvent>(NodeAction stateTransition, TEvent eventToPublish, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        ArgumentNullException.ThrowIfNull(stateTransition, nameof(stateTransition));
        ArgumentNullException.ThrowIfNull(eventToPublish, nameof(eventToPublish));

        if (cancellationToken.IsCancellationRequested) return;

        await States.DispatchAsync(stateTransition);
        await EmitEventAsync(eventToPublish, cancellationToken);
    }

    
    private static IMachineDefinition InitStatesDefinition => MachineDefinitionBuilder.Create()
            .InitialState(NodeState.Idle)
                .OnAction(NodeAction.Init).ToState(NodeState.Ready)
                .Build()
            .AddState(NodeState.Ready)
                .OnAction(NodeAction.Execute).ToState(NodeState.Running)
                .Build()
            .AddState(NodeState.Running)
                .OnAction(NodeAction.Pause).ToState(NodeState.Paused)
                .OnAction(NodeAction.Cancel).ToState(NodeState.Cancelled)
                .OnAction(NodeAction.Complete).ToState(NodeState.Completed)
                .OnAction(NodeAction.Fail).ToState(NodeState.Failed)
                .Build()
            .AddState(NodeState.Paused)
                .OnAction(NodeAction.Resume).ToState(NodeState.Running)
                .Build()
            .AddState(NodeState.Cancelled)
                .Build()
            .AddState(NodeState.Completed)
                .Build()
            .AddState(NodeState.Failed)
                .Build()
            .BuildDefinition();

    protected void EmitNodeEvent(string eventName, object? data = null)
    {
        var @event = new NodeEvent(eventName, Key.Name, data, "NodeBase");
        EmitEventAsync(@event).Wait();
    }
}

public record NodeEvent(string EventName, string NodeName, object? Data, string Caller) : Event("NodeEvent", Caller);