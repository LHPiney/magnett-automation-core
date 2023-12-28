using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Runtimes.Implementations;

public class NodeTest
{
    private const string Name = "name";
        
    [Fact]
    public void Ctor_WhenNameIsInformedAsString_ShouldReturnKeyProperInformed()
    {
        var instance = new NodeFake(Name);
        
        Assert.NotNull(instance.Key);
        Assert.Equal(Name, instance.Key.Name);
    }
    
    [Fact]
    public void Ctor_WhenNameIsInformedAsCommonNamedKey_ShouldReturnKeyProperInformed()
    {
        var key = CommonNamedKey.Create(Name);
        
        var instance = new NodeFake(key);
        
        Assert.NotNull(instance.Key);
        Assert.Equal(key.Name, instance.Key.Name);
    }
}