using Xunit;
using Magnett.Automation.Server;

namespace Magnett.Automation.Server.UnitTest;

public class FlowStateTest
{
    [Fact]
    public void Running_WhenGet_ReturnCorrectState()
    {
        var state = FlowState.Running;
        
        Assert.NotNull(state);
        Assert.Equal(1, state.Id);
        Assert.Equal("Running", state.Name);
    }

    [Fact]
    public void Paused_WhenGet_ReturnCorrectState()
    {
        var state = FlowState.Paused;
        
        Assert.NotNull(state);
        Assert.Equal(2, state.Id);
        Assert.Equal("Paused", state.Name);
    }

    [Fact]
    public void Completed_WhenGet_ReturnCorrectState()
    {
        var state = FlowState.Completed;
        
        Assert.NotNull(state);
        Assert.Equal(3, state.Id);
        Assert.Equal("Completed", state.Name);
    }

    [Fact]
    public void Failed_WhenGet_ReturnCorrectState()
    {
        var state = FlowState.Failed;
        
        Assert.NotNull(state);
        Assert.Equal(4, state.Id);
        Assert.Equal("Failed", state.Name);
    }

    [Fact]
    public void Equality_WhenCompareSameInstances_AreEqual()
    {
        var state1 = FlowState.Running;
        var state2 = FlowState.Running;
        
        Assert.Equal(state1, state2);
        Assert.True(state1 == state2);
    }

    [Fact]
    public void Inequality_WhenCompareDifferentStates_AreNotEqual()
    {
        var running = FlowState.Running;
        var paused = FlowState.Paused;
        
        Assert.NotEqual(running, paused);
        Assert.True(running != paused);
    }
}

