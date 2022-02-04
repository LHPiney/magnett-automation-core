using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Builders;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;

internal static class OrderStateDefinition
{
    public static IMachineDefinition Definition { get; }
            
    static OrderStateDefinition()
    {
        Definition = MachineDefinitionBuilder.Create()
                
            .InitialState(State.New)
            .OnAction(Action.Validate).ToState(State.Pending)
            .Build()
                
            .AddState(State.Pending)
            .OnAction(Action.Confirm).ToState(State.Confirmed)
            .OnAction(Action.Cancel).ToState(State.Cancelled)
            .Build()
            
            .AddState(State.Confirmed).Build()
            
            .AddState(State.Cancelled).Build()
            
            .BuildDefinition();
    }
    public class State : Enumeration
    {
        public static readonly State New = new State(1, nameof(New));
        public static readonly State Pending = new State(2, nameof(Pending));
        public static readonly State Confirmed = new State(3, nameof(Confirmed));
        public static readonly State Cancelled = new State(4, nameof(Cancelled));
        
        private State(int id, string name) : base(id, name)
        {
        }
    }

    public class Action : Enumeration
    {
        public static readonly Action Validate = new Action(1, nameof(Validate));
        public static readonly Action Confirm = new Action(2, nameof(Confirm));
        public static readonly Action Cancel = new Action(3, nameof(Cancel));
            
        private Action(int id, string name) : base(id, name)
        {
        }
    }
}