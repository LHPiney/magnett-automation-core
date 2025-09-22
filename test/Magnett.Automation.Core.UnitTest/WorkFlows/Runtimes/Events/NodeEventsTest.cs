using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Runtimes.Events;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Runtimes.Events;

public class NodeEventsTest
{
    private const string NodeName = "TestNode";
    private const string Code = "TEST_CODE";
    private const string Data = "TestData";
    private static readonly CommonNamedKey NodeKey = CommonNamedKey.Create(NodeName);

    #region OnNodeCancelledEvent Tests
    [Fact]
    public void OnNodeCancelledEvent_Create_WithValidParameters_ReturnsValidInstance()
    {
        var @event = OnNodeCancelledEvent.Create(NodeKey, Code, Data, "TestCaller");

        Assert.NotNull(@event);
        Assert.Equal(NodeName, @event.NodeName.Name);
        Assert.Equal(Code, @event.Code);
        Assert.Equal(Data, @event.Data);
        Assert.Equal("OnNodeCancelledEvent", @event.Name);
        Assert.Equal("TestCaller", @event.Caller);
    }

    [Fact]
    public void OnNodeCancelledEvent_Constructor_WithValidParameters_ReturnsValidInstance()
    {
        var @event = new OnNodeCancelledEvent("OnNodeCancelledEvent", "TestCaller", NodeKey, Code, Data);

        Assert.NotNull(@event);
        Assert.Equal(NodeName, @event.NodeName.Name);
        Assert.Equal(Code, @event.Code);
        Assert.Equal(Data, @event.Data);
        Assert.Equal("OnNodeCancelledEvent", @event.Name);
        Assert.Equal("TestCaller", @event.Caller);
    }

    [Fact]
    public void OnNodeCancelledEvent_Create_WithDefaultCaller_UsesCallerMemberName()
    {
        var @event = OnNodeCancelledEvent.Create(NodeKey, Code, Data);

        Assert.NotNull(@event);
        Assert.Equal(NodeName, @event.NodeName.Name);
        Assert.Equal(Code, @event.Code);
        Assert.Equal(Data, @event.Data);
        Assert.Equal("OnNodeCancelledEvent", @event.Name);
        Assert.Equal("OnNodeCancelledEvent_Create_WithDefaultCaller_UsesCallerMemberName", @event.Caller);
    }
    #endregion

    #region OnNodePausedEvent Tests
    [Fact]
    public void OnNodePausedEvent_Create_WithValidParameters_ReturnsValidInstance()
    {
        var @event = OnNodePausedEvent.Create(NodeKey, Code, Data, "TestCaller");

        Assert.NotNull(@event);
        Assert.Equal(NodeName, @event.NodeName.Name);
        Assert.Equal(Code, @event.Code);
        Assert.Equal(Data, @event.Data);
        Assert.Equal("OnNodePausedEvent", @event.Name);
        Assert.Equal("TestCaller", @event.Caller);
    }

    [Fact]
    public void OnNodePausedEvent_Constructor_WithValidParameters_ReturnsValidInstance()
    {
        var @event = new OnNodePausedEvent("OnNodePausedEvent", "TestCaller", NodeKey, Code, Data);

        Assert.NotNull(@event);
        Assert.Equal(NodeName, @event.NodeName.Name);
        Assert.Equal(Code, @event.Code);
        Assert.Equal(Data, @event.Data);
        Assert.Equal("OnNodePausedEvent", @event.Name);
        Assert.Equal("TestCaller", @event.Caller);
    }

    [Fact]
    public void OnNodePausedEvent_Create_WithDefaultCaller_UsesCallerMemberName()
    {
        var @event = OnNodePausedEvent.Create(NodeKey, Code, Data);

        Assert.NotNull(@event);
        Assert.Equal(NodeName, @event.NodeName.Name);
        Assert.Equal(Code, @event.Code);
        Assert.Equal(Data, @event.Data);
        Assert.Equal("OnNodePausedEvent", @event.Name);
        Assert.Equal("OnNodePausedEvent_Create_WithDefaultCaller_UsesCallerMemberName", @event.Caller);
    }
    #endregion
}
