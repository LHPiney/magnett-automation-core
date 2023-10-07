using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows;

public class NodeLinkKeyTest
{
   private const string Code = "code";
    
   private static readonly CommonNamedKey NodeKey = CommonNamedKey.Create("from");
    
    [Fact]
    public void Create_WhenNodeKeyIsNull_ThrowNewArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            NodeLinkKey.Create(null, Code));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_WhenCodeIsNullOrEmpty_ThrowNewArgumentNullException(string code)
    {
        Assert.Throws<ArgumentNullException>(() =>
            NodeLinkKey.Create(NodeKey, code));
    }

    [Fact]
    public void Create_WhenParametersAreValid_ReturnNodeLinkKeyValidInstance()
    {
        var expectedName = $"{NodeKey?.Name}.{Code}";

        var instance = NodeLinkKey.Create(NodeKey, Code);
        
        Assert.NotNull(instance);
        Assert.Equal(expectedName, instance.Name);
    }
}