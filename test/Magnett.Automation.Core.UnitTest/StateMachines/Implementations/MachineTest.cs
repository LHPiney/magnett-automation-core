using System;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Exceptions;
using Magnett.Automation.Core.StateMachines.Implementations;
using Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;
using Moq;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.StateMachines.Implementations;

public class MachineTest
{
    private static readonly CommonNamedKey InitialStateKey  = 
        CommonNamedKey.Create("InitialState");
    private static readonly CommonNamedKey FinalStateKey    = 
        CommonNamedKey.Create("FinalState");
    private static readonly CommonNamedKey NotFoundStateKey = 
        CommonNamedKey.Create("NotFound");
        
    [Fact]
    public async Task Create_When_Invoke_Return_Instance()
    {
        var definition = new Mock<IMachineDefinition>();
        var state      = new Mock<IState>();

        definition
            .Setup(def => def.InitialState)
            .Returns(state.Object);
            
        var instance = await Machine.CreateAsync(definition.Object);
            
        Assert.NotNull(instance);
    }
        
    [Fact]
    public async Task Create_When_Invoke_With_Null_Definition_Throw_Exception()
    {
        await Assert.ThrowsAsync<ArgumentNullException>( async () =>
            await Machine.CreateAsync(null));
    }
        
    [Fact]
    public async Task Create_When_Invoke_Without_InitialState_Throw_Exception()
    {
        var definition = new Mock<IMachineDefinition>();

        definition
            .Setup(def => def.InitialState)
            .Returns<IState>(null);
            
        await Assert.ThrowsAsync<InvalidMachineDefinitionException>(async () =>
           await Machine.CreateAsync(definition.Object));
    }
        
    [Fact]
    public async Task DispatchByString_When_Transition_Has_Unknown_State_Throw_Exception()
    {
        var definition   = new Mock<IMachineDefinition>();
        var transition   = new Mock<ITransition>();
        var initialState = new Mock<IState>();

        transition
            .Setup(tra => tra.ToStateKey)
            .Returns(NotFoundStateKey);

        initialState
            .Setup(sta => sta.ManageAction(It.IsAny<CommonNamedKey>()))
            .Returns(transition.Object);
            
        definition
            .Setup(def => def.InitialState)
            .Returns(initialState.Object);

        definition
            .Setup(def => 
                def.HasState(It.Is<CommonNamedKey>(val => val.Equals(NotFoundStateKey))))
            .Returns(false);

        var machine = await Machine.CreateAsync(definition.Object);
            
        await Assert.ThrowsAsync<StateNotFoundException>(async () =>
            await machine.DispatchAsync("AnyAction"));
    }

    [Fact] 
    public async Task DispatchByString_When_Transition_Is_Valid_Change_State()
    {
        var definition   = new Mock<IMachineDefinition>();
        var transition   = new Mock<ITransition>();
        var initialState = new Mock<IState>();
        var finalState   = new Mock<IState>();
            
        transition
            .Setup(tra => tra.ToStateKey)
            .Returns(FinalStateKey);

        initialState
            .Setup(sta => sta.ManageAction(It.IsAny<CommonNamedKey>()))
            .Returns(transition.Object);
            
        initialState
            .Setup(sta => sta.Key)
            .Returns(InitialStateKey);
            
        finalState
            .Setup(sta => sta.Key)
            .Returns(FinalStateKey);
            
        definition
            .Setup(def => def.InitialState)
            .Returns(initialState.Object);

        definition
            .Setup(def => 
                def.HasState(It.Is<CommonNamedKey>(val => val.Equals(FinalStateKey))))
            .Returns(true);
            
        definition
            .Setup(def => 
                def.GetState(It.Is<CommonNamedKey>(val => val.Equals(FinalStateKey))))
            .Returns(finalState.Object);
            
        var machine = await Machine.CreateAsync(definition.Object);
                
        var firstStateName = machine.Current.Key;
        await machine.DispatchAsync("AnyAction");
        var secondStateName = machine.Current.Key;
            
        Assert.NotNull(machine.Current);
        Assert.NotEqual(firstStateName, secondStateName);
        Assert.Equal(FinalStateKey, secondStateName);
    }
        
    [Fact] 
    public async Task DispatchByEnumeration_When_Transition_Has_Unknown_State_Throw_Exception()
    {
        var definition   = new Mock<IMachineDefinition>();
        var transition   = new Mock<ITransition>();
        var initialState = new Mock<IState>();

        transition
            .Setup(tra => tra.ToStateKey)
            .Returns(NotFoundStateKey);

        initialState
            .Setup(sta => sta.ManageAction(It.IsAny<CommonNamedKey>()))
            .Returns(transition.Object);
            
        definition
            .Setup(def => def.InitialState)
            .Returns(initialState.Object);

        definition
            .Setup(def => 
                def.HasState(It.Is<CommonNamedKey>(val => val.Equals(NotFoundStateKey))))
            .Returns(false);

         var machine = await Machine.CreateAsync(definition.Object);
            
        await Assert.ThrowsAsync<StateNotFoundException>(async () =>
            await machine.DispatchAsync(EnumerationFake.Create(1, "AnyAction")));
    }

    [Fact] public async Task DispatchByEnumeration_When_Transition_Is_Valid_Change_State()
    {
        var definition   = new Mock<IMachineDefinition>();
        var transition   = new Mock<ITransition>();
        var initialState = new Mock<IState>();
        var finalState   = new Mock<IState>();
            
        transition
            .Setup(tra => tra.ToStateKey)
            .Returns(FinalStateKey);

        initialState
            .Setup(sta => sta.ManageAction(It.IsAny<CommonNamedKey>()))
            .Returns(transition.Object);
            
        initialState
            .Setup(sta => sta.Key)
            .Returns(InitialStateKey);
            
        finalState
            .Setup(sta => sta.Key)
            .Returns(FinalStateKey);
            
        definition
            .Setup(def => def.InitialState)
            .Returns(initialState.Object);

        definition
            .Setup(def => 
                def.HasState(It.Is<CommonNamedKey>(val => val.Equals(FinalStateKey))))
            .Returns(true);
            
        definition
            .Setup(def => 
                def.GetState(It.Is<CommonNamedKey>(val => val.Equals(FinalStateKey))))
            .Returns(finalState.Object);
            
        var machine = await Machine.CreateAsync(definition.Object);
                
        var firstStateName = machine.Current.Key;
        await machine.DispatchAsync(EnumerationFake.Create(1, "AnyAction")) ;
        var secondStateName = machine.Current.Key;
            
        Assert.NotNull(machine.Current);
        Assert.NotEqual(firstStateName, secondStateName);
        Assert.Equal(FinalStateKey, secondStateName);
    }

    [Fact]
    public async Task EqualEnumeration_WhenEnumerationIsEqualToStateKey_ThenReturnTrue()
    {
        var definition   = new Mock<IMachineDefinition>();
        var initialState = new Mock<IState>();
            
        initialState
            .Setup(sta => sta.Key)
            .Returns(InitialStateKey);

        definition
            .Setup(def => def.InitialState)
            .Returns(initialState.Object);

        var machine = await Machine.CreateAsync(definition.Object);
        var isEqualToKey = machine.Equals(EnumerationFake.Create(1, InitialStateKey.Name));
        
        Assert.NotNull(machine.Current);
        Assert.True(isEqualToKey);
    }
        
    [Fact]
    public async Task EqualCommonKey_WhenEnumerationIsEqualToStateKey_ThenReturnTrue()
    {
        var definition   = new Mock<IMachineDefinition>();
        var initialState = new Mock<IState>();
            
        initialState
            .Setup(sta => sta.Key)
            .Returns(InitialStateKey);

        definition
            .Setup(def => def.InitialState)
            .Returns(initialState.Object);

        var machine = await Machine.CreateAsync(definition.Object);
        var isEqualToKey = machine.Equals(CommonNamedKey.Create(InitialStateKey.Name));
        
        Assert.NotNull(machine.Current);
        Assert.True(isEqualToKey);
    }
}