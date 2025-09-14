using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Builders;
using System;
using System.Threading.Tasks;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.SimpleMachine.Definitions;

public static class SimpleMachineDefinition
{
    private static IMachineDefinition _definition; 
            
    static SimpleMachineDefinition()
    {
        CreateDefinition();
    }

    public static async Task LogTransitionAsync(ITransition transition)
    {
        await Task.Delay(100); // Simula operación asíncrona
        Console.WriteLine($"Transitioned via {transition.ActionKey.Name} to {transition.ToStateKey.Name}");
    }

    private static void CreateDefinition()
    {
        _definition = MachineDefinitionBuilder.Create()
                
            .InitialState(State.Init)
            .OnAction(Action.Start)
                .OnCallBackAsync(LogTransitionAsync)
                .ToState(State.Working)
            .Build()
                
            .AddState(State.Working)
            .OnAction(Action.Pause)
                .OnCallBackAsync(LogTransitionAsync)
                .ToState(State.Paused)
            .OnAction(Action.Finish)
                .OnCallBackAsync(LogTransitionAsync)
                .ToState(State.Finished)
            .Build()
                
            .AddState(State.Paused)
            .OnAction(Action.Continue)
                .OnCallBackAsync(LogTransitionAsync)
                .ToState(State.Working)
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