using Xunit;

using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Implementations;

public class NodeAsyncTest
{
    private const string Name = "name";
        
    [Fact]
    public void Ctor_WhenNameIsInformedAsString_ShouldReturnKeyProperInformed()
    {
        var instance = new NodeAsyncFake(Name);
        
        Assert.NotNull(instance.Key);
        Assert.Equal(Name, instance.Key.Name);
    }
    
    [Fact]
    public void Ctor_WhenNameIsInformedAsCommonNamedKey_ShouldReturnKeyProperInformed()
    {
        var key = CommonNamedKey.Create(Name);
        
        var instance = new NodeAsyncFake(key);
        
        Assert.NotNull(instance.Key);
        Assert.Equal(key.Name, instance.Key.Name);
    }
}