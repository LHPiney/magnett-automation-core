using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Builders;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;

internal static class OrderDefinition
{
    public static IMachineDefinition Definition { get; }
            
    static OrderDefinition()
    {
        Definition = MachineDefinitionBuilder.Create()
                
            .InitialState(Status.New)
                .OnAction(Action.Validate).ToState(Status.Pending)
            .Build()
                
            .AddState(Status.Pending)
                .OnAction(Action.Confirm).ToState(Status.Confirmed)
                .OnAction(Action.Cancel).ToState(Status.Cancelled)
            .Build()
            
            .AddState(Status.Confirmed).Build()
            
            .AddState(Status.Cancelled).Build()
            
            .BuildDefinition();
    }

    public record Status : Enumeration
    {
        public static readonly Status New = new(1, nameof(New));
        public static readonly Status Pending = new(2, nameof(Pending));
        public static readonly Status Confirmed = new(3, nameof(Confirmed));
        public static readonly Status Cancelled = new(4, nameof(Cancelled));
        
        private Status(int id, string name) : base(id, name)
        {
        }
    }

    public record Action : Enumeration
    {
        public static readonly Action Validate = new(1, nameof(Validate));
        public static readonly Action Confirm = new(2, nameof(Confirm));
        public static readonly Action Cancel = new(3, nameof(Cancel));
            
        private Action(int id, string name) : base(id, name)
        {
        }
    }
}