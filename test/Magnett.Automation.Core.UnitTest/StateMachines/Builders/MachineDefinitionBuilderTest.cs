using Magnett.Automation.Core.StateMachines.Builders;
using Magnett.Automation.Core.UnitTest.Commons.Helpers;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.StateMachines.Builders;

public class MachineDefinitionBuilderTest
{
    private const string InitialStateName = "InitialState";
    private const string AnyStateName     = "AnyStateName";

    [Fact]
    public void Create_When_Invoke_Return_Instance()
    {
        var instance = MachineDefinitionBuilder.Create();

        Assert.NotNull(instance);
        Assert.IsType<MachineDefinitionBuilder>(instance);
    }

    [Fact]
    public void InitialState_When_Invoke_With_String_Parameter_Return_Initial_StateBuilder()
    {
        var stateDefinition = MachineDefinitionBuilder
            .Create()
            .InitialState(InitialStateName);
            
        Assert.NotNull(stateDefinition);
        Assert.IsType<StateBuilder>(stateDefinition);
        Assert.True(stateDefinition.IsInitialState);
    }
        
    [Fact]
    public void InitialState_When_Invoke_With_Enumeration_Parameter_Return_Initial_StateBuilder()
    {
        var stateEnumeration = new EnumerationMockup(1, InitialStateName);
        var stateDefinition = MachineDefinitionBuilder
            .Create()
            .InitialState(stateEnumeration);
            
        Assert.NotNull(stateDefinition);
        Assert.IsType<StateBuilder>(stateDefinition);
        Assert.True(stateDefinition.IsInitialState);
    }
        
    [Fact]
    public void AddState_When_Invoke_With_String_Parameter_Return_Not_Initial_StateBuilder()
    {
        var stateDefinition = MachineDefinitionBuilder
            .Create()
            .AddState(AnyStateName);
            
        Assert.NotNull(stateDefinition);
        Assert.IsType<StateBuilder>(stateDefinition);
        Assert.False(stateDefinition.IsInitialState);
    }
        
    [Fact]
    public void AddState_When_Invoke_With_Enumeration_Parameter_Return_Not_Initial_StateBuilder()
    {
        var stateEnumeration = new EnumerationMockup(1, AnyStateName);
        var stateDefinition = MachineDefinitionBuilder
            .Create()
            .AddState(stateEnumeration);
            
        Assert.NotNull(stateDefinition);
        Assert.IsType<StateBuilder>(stateDefinition);   
        Assert.False(stateDefinition.IsInitialState);
    }
}