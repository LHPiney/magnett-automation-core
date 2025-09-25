using System;
using Magnett.Automation.Core.UnitTest.Events.Fakes;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Events.Implementation;

public class EventTest
{
    [Fact]
    public void Create_When_Invoke_Without_Name_Throws_Exception()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new EventMock(null, "Caller"));
    }
    
    [Fact]
    public void Create_When_Invoke_Without_Caller_Store_Empty()
    {
        var eventInstance = new EventMock("Name", null);
        
        Assert.NotNull(eventInstance);
        Assert.Equal(string.Empty, eventInstance.Caller);
    }
    
    [Fact]
    public void Create_When_Invoke_Return_Instance()
    {
        var eventInstance = new EventMock("Name", "Caller");
        
        Assert.NotNull(eventInstance);
    }
    
    [Fact]
    public void Create_When_Name_Is_Good_Is_Proper_Stored()
    {
        var eventInstance = new EventMock("Name", "Caller");
        
        Assert.NotNull(eventInstance.Name);
        Assert.Equal("Name", eventInstance.Name);
    }
    
    [Fact]
    public void Create_When_Caller_Is_Good_Is_Proper_Stored()
    {
        var eventInstance = new EventMock("Name", "Caller");
        
        Assert.NotNull(eventInstance.Caller);
        Assert.Equal("Caller", eventInstance.Caller);
    }
    
    [Fact]
    public void Create_When_Id_Is_Good_Is_Proper_Stored()
    {
        var eventInstance = new EventMock("Name", "Caller");
        
        Assert.NotEqual(Guid.Empty, eventInstance.Id);
    }
    
    [Fact]
    public void Create_When_CreatedAt_Is_Good_Is_Proper_Stored()
    {
        var eventInstance = new EventMock("Name", "Caller");
        
        Assert.NotEqual(DateTime.MinValue, eventInstance.CreatedAt);
        Assert.True(eventInstance.CreatedAt < DateTime.UtcNow);
    }
}