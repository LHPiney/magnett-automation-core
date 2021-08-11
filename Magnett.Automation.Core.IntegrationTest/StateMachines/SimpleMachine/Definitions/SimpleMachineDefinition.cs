using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Builders;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.SimpleMachine.Definitions
{
    public static class SimpleMachineDefinition
    {
        private static IMachineDefinition _definition; 
            
        static SimpleMachineDefinition()
        {
            CreateDefinition();
        }
        
        private static void CreateDefinition()
        {
            _definition = MachineDefinitionBuilder.Create()
                .InitialState(State.Init)
                    .OnAction(Action.Start).ToState(State.Working)
                    .Build()
                .AddState(State.Working)
                    .OnAction(Action.Pause).ToState(State.Paused)
                    .OnAction(Action.Finish).ToState(State.Finished)
                    .Build()
                .AddState(State.Paused)
                    .OnAction(Action.Continue).ToState(State.Working)
                    .Build()
                .AddState(State.Finished)
                    .Build()
                .BuildDefinition();
        }

        public static IMachineDefinition GetDefinition()
        {
            return _definition;
        }
    }
}