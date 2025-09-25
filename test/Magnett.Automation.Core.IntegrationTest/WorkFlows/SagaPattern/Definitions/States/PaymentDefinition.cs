using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Builders;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.States;

internal static class PaymentDefinition
{
    public static IMachineDefinition Definition { get; }

    static PaymentDefinition()
    {
        Definition = MachineDefinitionBuilder.Create()
                
            .InitialState(Status.New)
                .OnAction(Action.PreAuthorize).ToState(Status.PreAuthorized)
                .OnAction(Action.Deny).ToState(Status.Denied)
            .Build()
            
            .AddState(Status.PreAuthorized)
                .OnAction(Action.Confirm).ToState(Status.Confirmed)
                .OnAction(Action.Cancel).ToState(Status.Cancelled)
            .Build()
            
            .AddState(Status.Denied)
                .OnAction(Action.Cancel).ToState(Status.Cancelled)
            .Build()
            
            .AddState(Status.Confirmed).Build()
            
            .AddState(Status.Cancelled).Build()
            
            .BuildDefinition();
    }
    
    public record Action : Enumeration
    {
        public static readonly Action PreAuthorize = new(1, nameof(PreAuthorize));
        public static readonly Action Deny = new(2, nameof(Deny));
        public static readonly Action Confirm = new(3, nameof(Confirm));
        public static readonly Action Cancel = new(4, nameof(Cancel));
            
        private Action(int id, string name) : base(id, name)
        {
        }
    }

    public record Status : Enumeration
    {
        public static readonly Status New = new(1, nameof(New));
        public static readonly Status PreAuthorized = new(2, nameof(PreAuthorized));
        public static readonly Status Denied = new(3, nameof(Denied));
        public static readonly Status Confirmed = new(4, nameof(Confirmed));
        public static readonly Status Cancelled = new(5, nameof(Cancelled));

        private Status(int id, string name) : base(id, name)
        {
        }
    }
}