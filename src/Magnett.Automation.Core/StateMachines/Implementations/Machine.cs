using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.StateMachines.Events;

namespace Magnett.Automation.Core.StateMachines.Implementations;

/// <summary>
/// Represents a state machine IMachine <seealso cref="IMachine"/> default's implementation
/// that handles transitions between states
/// based on provided actions and maintains the current state.
/// If IEventBus is provided, it will be used to publish events.
/// </summary>
public class Machine : EventEmitterEntity, IMachine
{
    private IState State { get; set; }
    private IMachineDefinition Definition { get; }
    public IState Current => State;

    protected Machine(IMachineDefinition definition, IEventBus eventBus) : base(eventBus)
    {
        Id = Guid.NewGuid();
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
    }

    protected async Task<IMachine> InitializeAsync()
    {
        State = Definition.InitialState;

        await EmitEventAsync(OnMachineInit.Create(Id));

        return this;
    }

    private async Task Transit(ITransition transition)
    {
        var newState = Definition.HasState(transition.ToStateKey)
            ? Definition.GetState(transition.ToStateKey)
            : throw new StateNotFoundException(transition.ToStateKey.Name);

        State = newState;
        
        if (transition.OnTransitionAsync != null)
            await transition.OnTransitionAsync.Invoke(transition);
    }

    # region IMachine
        
    public Guid Id { get; }

    public IState GetState(CommonNamedKey stateName)
    {
        ArgumentNullException.ThrowIfNull(stateName);
        
        return Definition.GetState(stateName);
    }
    
    public async Task<IMachine> DispatchAsync(Enumeration action)
    {
        ArgumentNullException.ThrowIfNull(action);
        
        return await DispatchAsync(action.Name);
    }
        
    public async Task<IMachine> DispatchAsync(string actionName)
    {
        ArgumentNullException.ThrowIfNull(actionName);
        
        if (State == null)
        { 
            throw new InvalidOperationException("Machine is not initialized");
        }
        
        var sourceState = State.Key;

        await Transit(State.ManageAction(CommonNamedKey.Create(actionName)));

        var targetState = State.Key;
        
        await EmitEventAsync(OnMachineChangeState.Create(Id, sourceState, targetState, actionName));
            
        return this;
    }
    
    public async Task<IMachine> ReStartAsync()
    {
        if (State == null)
        {
            throw new InvalidOperationException("Machine is not initialized");
        }
        
        await InitializeAsync();

        return this;
    }

    public IMachineDefinition GetDefinition()
    {
        return Definition;
    }

    public bool Equals(CommonNamedKey obj)
    {
        return State.Key.Equals(obj);
    }
        
    public bool Equals(Enumeration obj)
    {
        return State.Key.Equals(obj);
    }

    #endregion

    public static async Task<IMachine> CreateAsync(IMachineDefinition definition, IEventBus eventBus = null)
    {
        ArgumentNullException.ThrowIfNull(definition);
        
        if (definition.InitialState == null)
        {
            throw new InvalidMachineDefinitionException("Machine definition must have an initial state");
        }
        
        return await new Machine(definition, eventBus)
            .InitializeAsync();
    }
}