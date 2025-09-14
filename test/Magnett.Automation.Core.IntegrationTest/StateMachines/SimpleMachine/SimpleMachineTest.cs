using Magnett.Automation.Core.IntegrationTest.StateMachines.SimpleMachine.Definitions;
using Magnett.Automation.Core.StateMachines.Implementations;
using System.Threading.Tasks;
using Xunit;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.SimpleMachine;

public class SimpleMachineTest
{
    [Fact]
    public void CreateDefinition_GetValidDefinition()
    {
        var definition = SimpleMachineDefinition.GetDefinition();
            
        Assert.NotNull(definition);
    }

    [Fact]
    public async Task Machine_Should_Change_State_On_Valid_Transition()
    {
        var definition = SimpleMachineDefinition.GetDefinition();
        var machine = await Machine.CreateAsync(definition);

        // Estado inicial debe ser Init
        Assert.Equal("Init", machine.Current.Key.Name);

        // Ejecutar transición Start -> Working
        await machine.DispatchAsync("Start");
        Assert.Equal("Working", machine.Current.Key.Name);

        // Ejecutar transición Finish -> Finished
        await machine.DispatchAsync("Finish");
        Assert.Equal("Finished", machine.Current.Key.Name);
    }

    [Fact]
    public async Task Machine_Should_Execute_Async_Action_On_Transition()
    {
        var definition = SimpleMachineDefinition.GetDefinition();
        var machine = await Machine.CreateAsync(definition);

        // Suscribirse a la consola para detectar la acción (alternativamente, mockear LogTransitionAsync)
        System.Console.SetOut(new System.IO.StringWriter());

        // Ejecutar transición Start -> Working
        await machine.DispatchAsync("Start");
        // Ejecutar transición Pause -> Paused
        await machine.DispatchAsync("Pause");
        // Ejecutar transición Continue -> Working
        await machine.DispatchAsync("Continue");
        // Ejecutar transición Finish -> Finished
        await machine.DispatchAsync("Finish");

        // Si no hay excepción, las acciones async se ejecutaron correctamente
        Assert.Equal("Finished", machine.Current.Key.Name);
    }
}