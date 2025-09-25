using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Runtimes;

public class NodeStateTest
{
    #region WithName Tests
    [Fact]
    public void WithName_WhenNameIsNull_ReturnsIdle()
    {
        var result = NodeState.WithName(null);
        Assert.Equal(NodeState.Idle, result);
    }

    [Fact]
    public void WithName_WhenNameIsIdle_ReturnsIdle()
    {
        var key = CommonNamedKey.Create("Idle");
        var result = NodeState.WithName(key);
        Assert.Equal(NodeState.Idle, result);
    }

    [Fact]
    public void WithName_WhenNameIsReady_ReturnsReady()
    {
        var key = CommonNamedKey.Create("Ready");
        var result = NodeState.WithName(key);
        Assert.Equal(NodeState.Ready, result);
    }

    [Fact]
    public void WithName_WhenNameIsRunning_ReturnsRunning()
    {
        var key = CommonNamedKey.Create("Running");
        var result = NodeState.WithName(key);
        Assert.Equal(NodeState.Running, result);
    }

    [Fact]
    public void WithName_WhenNameIsCompleted_ReturnsCompleted()
    {
        var key = CommonNamedKey.Create("Completed");
        var result = NodeState.WithName(key);
        Assert.Equal(NodeState.Completed, result);
    }

    [Fact]
    public void WithName_WhenNameIsFailed_ReturnsFailed()
    {
        var key = CommonNamedKey.Create("Failed");
        var result = NodeState.WithName(key);
        Assert.Equal(NodeState.Failed, result);
    }

    [Fact]
    public void WithName_WhenNameIsPaused_ReturnsPaused()
    {
        var key = CommonNamedKey.Create("Paused");
        var result = NodeState.WithName(key);
        Assert.Equal(NodeState.Paused, result);
    }

    [Fact]
    public void WithName_WhenNameIsCancelled_ReturnsCancelled()
    {
        var key = CommonNamedKey.Create("Cancelled");
        var result = NodeState.WithName(key);
        Assert.Equal(NodeState.Cancelled, result);
    }

    [Fact]
    public void WithName_WhenNameIsUnknown_ThrowsArgumentOutOfRangeException()
    {
        var key = CommonNamedKey.Create("UnknownState");
        Assert.Throws<ArgumentOutOfRangeException>(() => NodeState.WithName(key));
    }

    [Fact]
    public void WithName_WhenNameIsEmpty_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CommonNamedKey.Create(""));
    }

    [Fact]
    public void WithName_WhenNameIsWhitespace_ThrowsArgumentOutOfRangeException()
    {
        var key = CommonNamedKey.Create("   ");
        Assert.Throws<ArgumentOutOfRangeException>(() => NodeState.WithName(key));
    }
    #endregion

    #region Static Properties Tests
    [Fact]
    public void StaticProperties_AllStatesAreNotNull()
    {
        Assert.NotNull(NodeState.Idle);
        Assert.NotNull(NodeState.Ready);
        Assert.NotNull(NodeState.Running);
        Assert.NotNull(NodeState.Completed);
        Assert.NotNull(NodeState.Failed);
        Assert.NotNull(NodeState.Paused);
        Assert.NotNull(NodeState.Cancelled);
    }

    [Fact]
    public void StaticProperties_AllStatesHaveCorrectNames()
    {
        Assert.Equal("Idle", NodeState.Idle.Name);
        Assert.Equal("Ready", NodeState.Ready.Name);
        Assert.Equal("Running", NodeState.Running.Name);
        Assert.Equal("Completed", NodeState.Completed.Name);
        Assert.Equal("Failed", NodeState.Failed.Name);
        Assert.Equal("Paused", NodeState.Paused.Name);
        Assert.Equal("Cancelled", NodeState.Cancelled.Name);
    }

    [Fact]
    public void StaticProperties_AllStatesHaveCorrectIds()
    {
        Assert.Equal(0, NodeState.Idle.Id);
        Assert.Equal(1, NodeState.Ready.Id);
        Assert.Equal(2, NodeState.Running.Id);
        Assert.Equal(3, NodeState.Completed.Id);
        Assert.Equal(4, NodeState.Failed.Id);
        Assert.Equal(5, NodeState.Paused.Id);
        Assert.Equal(6, NodeState.Cancelled.Id);
    }
    #endregion
}
