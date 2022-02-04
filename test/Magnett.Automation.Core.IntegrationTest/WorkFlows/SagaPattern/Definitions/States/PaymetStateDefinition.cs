using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Builders;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;

internal static class PaymentStateDefinition
{
    private static IMachineDefinition _definition; 
            
    static PaymentStateDefinition()
    {
        CreateDefinition();
    }
    public class State : Enumeration
    {
        public static readonly State New = new State(1, nameof(New));
        public static readonly State PreAuthorized = new State(2, nameof(PreAuthorized));
        public static readonly State Denied = new State(3, nameof(Denied));
        public static readonly State Confirmed = new State(4, nameof(Confirmed));
        public static readonly State Cancelled = new State(5, nameof(Cancelled));
        
        private State(int id, string name) : base(id, name)
        {
        }
    }

    public class Action : Enumeration
    {
        public static readonly Action PreAuthorize = new Action(1, nameof(PreAuthorize));
        public static readonly Action Deny = new Action(2, nameof(Deny));
        public static readonly Action Confirm = new Action(3, nameof(Confirm));
        public static readonly Action Cancel = new Action(4, nameof(Cancel));
            
        private Action(int id, string name) : base(id, name)
        {
        }
    }

    private static void CreateDefinition()
    {
        _definition = MachineDefinitionBuilder.Create()
                
            .InitialState(State.New)
              .OnAction(Action.PreAuthorize).ToState(State.PreAuthorized)
              .OnAction(Action.Deny).ToState(State.Denied)
            .Build()
                
            .AddState(State.PreAuthorized)
                .OnAction(Action.Confirm).ToState(State.Confirmed)
                .OnAction(Action.Cancel).ToState(State.Cancelled)
            .Build()

            .AddState(State.Denied)
                .OnAction(Action.Cancel).ToState(State.Cancelled)
            .Build()
            
            .AddState(State.Confirmed).Build()
            
            .AddState(State.Cancelled).Build()
            
            .BuildDefinition();
    }
    
    public static IMachineDefinition GetDefinition()
    {
        return _definition;
    }
}