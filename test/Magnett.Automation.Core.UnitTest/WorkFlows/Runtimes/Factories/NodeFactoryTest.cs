using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;
using Magnett.Automation.Core.WorkFlows.Definitions.Implementations;
using Xunit;

using Magnett.Automation.Core.WorkFlows.Runtimes.Factories;
using Moq;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Runtimes.Factories;

public class NodeFactoryTest
{
    [Fact]
    public void Ctor_WhenNodeDefinitionIsNull_ThrowsArgumentNullException()
    {
        var eventBus = new Mock<IEventBus>();
        Assert.Throws<ArgumentNullException>(() => 
            NodeFactory.CreateNode(null, eventBus.Object));
    }
    
    [Fact]
    public void Ctor_WhenNodeTypeDoesNotHaveValidConstructor_ThrowsInvalidOperationException()
    {
        var eventBus = new Mock<IEventBus>();
        var nodeDefinition = NodeDefinition
            .Create<InvalidNodeFake>(CommonNamedKey.Create("test"));

        Assert.Throws<InvalidOperationException>(() => 
            NodeFactory.CreateNode(nodeDefinition, eventBus.Object));
    }
}