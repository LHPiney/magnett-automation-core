using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Definitions;

public class NodeExitTest
{
    private const string GoodCode = "Ok";
    private const string Data = "Data Info";

    private static readonly Enumeration GoodEnumerationCode =  EnumerationFake.Create(1, GoodCode);

    #region Completed with Enumeration
    [Fact]
    public void CreateAsEnumeration_WhenCodeIsNull_ThrowException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _ = NodeExit.Completed((Enumeration) null));
    }
        
    [Fact]
    public void CreateAsEnumeration_WhenCodeNotIsNull_ReturnValidInstance()
    {
        var instance = NodeExit.Completed(GoodEnumerationCode);
            
        Assert.NotNull(instance);
        Assert.Equal(GoodCode, instance.Code);
    }
        
    [Fact]
    public void CreateAsEnumeration_WhenIsErrorIsInformed_ValueIsCorrectlyStored()
    {
        var instance = NodeExit.Failed(GoodEnumerationCode);
            
        Assert.NotNull(instance);
        Assert.True(instance.State == ExitState.Failed);
    }
        
    [Fact]
    public void CreateAsEnumeration_WhenDataIsInformed_ValueIsCorrectlyStored()
    {
        var instance = NodeExit.Completed(GoodEnumerationCode, Data);
            
        Assert.NotNull(instance);
        Assert.Equal(Data, instance.Data);
    }
    #endregion

    #region Completed with String
    [Fact]
    public void CreateAsString_WhenCodeIsNull_ThrowException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _ = NodeExit.Completed((string) null));
    }
        
    [Fact]
    public void CreateAsString_WhenCodeNotIsNull_ReturnValidInstance()
    {
        var instance = NodeExit.Completed(GoodCode);
            
        Assert.NotNull(instance);
        Assert.Equal(GoodCode, instance.Code);
    }
        
    [Fact]
    public void CreateAsString_WhenIsErrorIsInformed_ValueIsCorrectlyStored()
    {
        var instance = NodeExit.Failed(GoodCode);
            
        Assert.NotNull(instance);
        Assert.True(instance.State == ExitState.Failed);
    }
        
    [Fact]
    public void CreateAsString_WhenDataIsInformed_ValueIsCorrectlyStored()
    {
        var instance = NodeExit.Completed(GoodCode, Data);
            
        Assert.NotNull(instance);
        Assert.Equal(Data, instance.Data);
    }
    #endregion
    
    #region Completed paused with String
    [Fact]
    public void PausedAsString_WhenCodeIsNull_ThrowException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _ = NodeExit.Paused((string) null));
    }
        
    [Fact]
    public void PausedString_WhenCodeNotIsNull_ReturnValidInstance()
    {
        var instance = NodeExit.Paused(GoodEnumerationCode, Data);
            
        Assert.NotNull(instance);
        Assert.Equal(GoodCode, instance.Code);
        Assert.True(instance.State == ExitState.Paused);
    }
        
    [Fact]
    public void PausedAsString_WhenDataIsInformed_ValueIsCorrectlyStored()
    {
        var instance = NodeExit.Paused(GoodCode, Data);
            
        Assert.NotNull(instance);
        Assert.Equal(Data, instance.Data);
        Assert.True(instance.State == ExitState.Paused);
    }
    #endregion
    
    #region Completed paused with Enumeration
    [Fact]
    public void PausedAsEnumeration_WhenCodeIsNull_ThrowException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _ = NodeExit.Paused((Enumeration) null));
    }
        
    [Fact]
    public void PausedAsEnumeration_WhenCodeNotIsNull_ReturnValidInstance()
    {
        var instance = NodeExit.Paused(GoodEnumerationCode);
            
        Assert.NotNull(instance);
        Assert.Equal(GoodCode, instance.Code);
        Assert.True(instance.State == ExitState.Paused);
    }
        
    [Fact]
    public void PausedAsEnumeration_WhenDataIsInformed_ValueIsCorrectlyStored()
    {
        var instance = NodeExit.Paused(GoodEnumerationCode, Data);
            
        Assert.NotNull(instance);
        Assert.Equal(Data, instance.Data);
        Assert.True(instance.State == ExitState.Paused);
    }
    #endregion
}