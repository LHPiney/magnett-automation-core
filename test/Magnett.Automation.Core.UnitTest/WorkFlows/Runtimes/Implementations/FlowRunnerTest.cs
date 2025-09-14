using System;
using Moq;
using Xunit;

using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Definitions.Implementations;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Runtimes.Implementations;

public class FlowRunnerTest
{
    private static readonly CommonNamedKey InitialNodeKey = CommonNamedKey.Create("InitialNode");
    private static readonly CommonNamedKey SecondNodeKey  = CommonNamedKey.Create("SecondNode");

    [Fact]
    public void Create_WhenDefinitionIsNull_ThrowException()
    {
        var eventBus = new Mock<IEventBus>();   
        
        Assert.Throws<ArgumentNullException>(() =>
            FlowRunner.Create(null, Context.Create(eventBus.Object)));
    }
        
    [Fact]
    public void Create_WhenContextIsNull_ThrowException()
    {
        var definition = new Mock<IFlowDefinition>();
            
        Assert.Throws<ArgumentNullException>(() =>
            FlowRunner.Create(definition.Object, null));
    }
        
    [Fact]
    public void Create_WhenDefinitionIsValid_ReturnInstance()
    {
        var eventBus = new Mock<IEventBus>();   
        var definition = new Mock<IFlowDefinition>();
        definition.Setup(mock => mock.InitialNode)
            .Returns(InitialNodeKey);
        definition.Setup(mock => mock.GetNode(InitialNodeKey))
            .Returns(NodeDefinition.Create<NodeFake>(InitialNodeKey));
            
        var instance = FlowRunner.Create(definition.Object, Context.Create(eventBus.Object));
            
        Assert.NotNull(instance);
        Assert.IsAssignableFrom<IFlowRunner>(instance);
    }
}