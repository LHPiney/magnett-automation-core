namespace Magnett.Automation.Core.StateMachines.Implementations;

/// <summary>
/// Represents default's implementation
/// definition for a state machine. Provides functionality
/// to manage states and verify if they exist within the machine.
/// </summary>
internal class MachineDefinition : IMachineDefinition
{
    private readonly StateList _stateList; 
        
    private MachineDefinition(
        IState initialState, 
        StateList stateList)
    {
        InitialState = initialState ??
                       throw new ArgumentNullException(nameof(initialState));
                           
        _stateList = stateList ??
                     throw new ArgumentNullException(nameof(stateList));
    }
        
    #region IMachineDefinition

    public IState InitialState { get; }
        
    public bool HasState(CommonNamedKey stateKey)
    {
        return _stateList.HasItem(stateKey);
    }

    public IState GetState(CommonNamedKey stateKey)
    {
        return _stateList.Get(stateKey);
    }
    
    #endregion
        
    /// <summary>
    /// Creates a new MachineDefinition instance with the specified initial state and state list.
    /// </summary>
    /// <param name="initialState">The initial state of the machine.</param>
    /// <param name="stateList">The list of all states in the machine.</param>
    /// <returns>A new MachineDefinition instance.</returns>
    public static IMachineDefinition Create(
        IState initialState,
        StateList stateList)
    {
        return new MachineDefinition(initialState, stateList);
    }
}