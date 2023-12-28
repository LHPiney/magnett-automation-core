using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Definitions.Implementations;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Definitions;

public class NodeLinkTest
{
    private const string Code = "code";
    
    private static readonly CommonNamedKey From = CommonNamedKey.Create("from");
    private static readonly CommonNamedKey To   = CommonNamedKey.Create("to");
    
    [Fact]
    public void Create_WhenFromNodeKeyIsNull_ThrowNewArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            NodeLink.Create(null, To, Code));
    }
    
    [Fact]
    public void Create_WhenToNodeKeyIsNull_ThrowNewArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            NodeLink.Create(From, null, Code));
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_WhenCodeIsNullOrEmpty_ThrowNewArgumentNullException(string code)
    {
        Assert.Throws<ArgumentNullException>(() =>
            NodeLink.Create(From, null, code));
    }

    [Fact]
    public void Create_WhenParametersAreValid_ReturnINodeLinkValidInstance()
    {
        var instance = NodeLink.Create(From, To, Code);
        
        Assert.NotNull(instance);
        Assert.Equal(From, instance.FromNodeKey);
        Assert.Equal(To, instance.ToNodeKey);
        Assert.NotNull(instance.Key);
        Assert.Equal(Code, instance.Code);
    }
}