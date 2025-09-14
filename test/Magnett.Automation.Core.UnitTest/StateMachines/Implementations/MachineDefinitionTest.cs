using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Collections;
using Magnett.Automation.Core.StateMachines.Implementations;
using Moq;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.StateMachines.Implementations;

public class MachineDefinitionTest
{
    private static readonly CommonNamedKey InitialStateKey = CommonNamedKey.Create("InitialState");

    #region Completed
    [Fact]
    public void Create_When_Invoke_Without_InitialState_Throws_Exception()
    {
        var stateList    = new Mock<StateList>();
            
        Assert.Throws<ArgumentNullException>(()=>
            MachineDefinition.Create(null, stateList.Object));
            
    }
        
    [Fact]
    public void Create_When_Invoke_Without_StateList_Throws_Exception()
    {
        var initialState = new Mock<IState>();

        Assert.Throws<ArgumentNullException>(()=>
            MachineDefinition.Create(initialState.Object, null));
    }
        
    [Fact]
    public void Create_When_Invoke_Return_Instance()
    {
        var initialState = new Mock<IState>();
        var stateList    = new Mock<StateList>();
            
        var definition = MachineDefinition.Create(
            initialState.Object, 
            stateList.Object);
            
        Assert.NotNull(definition);
    }
        
    [Fact]
    public void Create_When_InitialState_Is_Good_Is_Proper_Stored()
    {
        var initialState = new Mock<IState>();
        var stateList    = new Mock<StateList>();

        var definition = MachineDefinition.Create(
            initialState.Object, 
            stateList.Object);

        Assert.NotNull(definition.InitialState);
        Assert.Equal(initialState.Object, definition.InitialState);
    }
        
    #endregion
        
    [Fact]
    public void HasState_When_Invoke_Call_StateList_HasItem()
    {
        var initialState = new Mock<IState>();
        var stateList    = new Mock<StateList>();
            
        var definition = MachineDefinition.Create(
            initialState.Object, 
            stateList.Object);

        _ = definition.HasState(InitialStateKey);
            
        stateList.Verify(
            dic =>  dic.HasItem(InitialStateKey), 
            Times.Once);
    }
        
    [Fact]
    public void GetState_When_Invoke_Call_StateList_Get()
    {
        var initialState = new Mock<IState>();
        var stateList    = new Mock<StateList>();
            
        var definition = MachineDefinition.Create(
            initialState.Object, 
            stateList.Object);

        stateList
            .Setup(list => list.Get(InitialStateKey))
            .Returns(initialState.Object);

        _ = definition.GetState(InitialStateKey);
            
        stateList.Verify(
            dic =>  dic.Get(InitialStateKey), 
            Times.Once);
    }
}