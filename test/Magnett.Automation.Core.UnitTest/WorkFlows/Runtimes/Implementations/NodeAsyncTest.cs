using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;
using Moq;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Runtimes.Implementations;

public class NodeAsyncTest
{
    private const string Name = "name";
        
    [Fact]
    public void Ctor_WhenNameIsInformedAsString_ShouldReturnKeyProperInformed()
    {
        var eventBus = new Mock<IEventBus>();
        var instance = new NodeAsyncFake(Name, eventBus.Object);
        
        Assert.NotNull(instance.Key);
        Assert.Equal(Name, instance.Key.Name);
    }
    
    [Fact]
    public void Ctor_WhenNameIsInformedAsCommonNamedKey_ShouldReturnKeyProperInformed()
    {
        var eventBus = new Mock<IEventBus>();
        var key = CommonNamedKey.Create(Name);
        
        var instance = new NodeAsyncFake(key, eventBus.Object);
        
        Assert.NotNull(instance.Key);
        Assert.Equal(key.Name, instance.Key.Name);
    }
}