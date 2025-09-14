using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Events;
using Moq;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Runtimes.Implementations;

public class NodeTest
{
    private const string Name = "name";
        
    [Fact]
    public void Ctor_WhenNameIsInformedAsString_ShouldReturnKeyProperInformed()
    {
        var eventBus = new Mock<IEventBus>();
        var instance = new NodeFake(Name, eventBus.Object);
        
        Assert.NotNull(instance.Key);
        Assert.Equal(Name, instance.Key.Name);
    }
    
    [Fact]
    public void Ctor_WhenNameIsInformedAsCommonNamedKey_ShouldReturnKeyProperInformed()
    {
        var eventBus = new Mock<IEventBus>();
        var key = CommonNamedKey.Create(Name);
        
        var instance = new NodeFake(key, eventBus.Object);
        
        Assert.NotNull(instance.Key);
        Assert.Equal(key.Name, instance.Key.Name);
    }

    [Fact]
    public async Task Execute_WhenNodeFails_ShouldEmitOnNodeFailedEvent()
    {
        var eventBus = new Mock<IEventBus>();
        eventBus
            .Setup(eb => eb.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var node = new NodeFailFake("FailingNode", eventBus.Object);
        await node.Init();

        var ctx = Context.Create(eventBus.Object);
        var result = await node.Execute(ctx);

        Assert.NotNull(result);
        Assert.Equal(ExitState.Failed, result.State);

        eventBus.Verify(
            eb => eb.PublishAsync(It.Is<IEvent>(e => e is OnNodeFailedEvent), It.IsAny<CancellationToken>()),
            Times.AtLeastOnce());
    }
}