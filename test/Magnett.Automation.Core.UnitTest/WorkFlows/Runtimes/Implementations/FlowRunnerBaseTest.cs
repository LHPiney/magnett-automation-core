using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Definitions.Implementations;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Runtimes.Implementations;

public class FlowRunnerBaseTest
{
    private static readonly CommonNamedKey InitialNodeKey = CommonNamedKey.Create("InitialNode");
    private static readonly CommonNamedKey SecondNodeKey  = CommonNamedKey.Create("SecondNode");
    
    private ILogger<FlowRunnerBaseFake> GetLogger()
    {
        using var factory = LoggerFactory.Create(builder => builder.AddConsole());
        return factory.CreateLogger<FlowRunnerBaseFake>();
    }
    
    #region Ctor
        
    [Fact]
    public void Ctor_WhenDefinitionIsNull_ThrowException()
    {
        var eventBus = new Mock<IEventBus>();   
        
        Assert.Throws<ArgumentNullException>(() =>
            new FlowRunnerBaseFake(null, Context.Create(eventBus.Object), eventBus.Object, GetLogger()));
    }
        
    [Fact]
    public void Ctor_WhenContextIsNull_ThrowException()
    {
        var eventBus = new Mock<IEventBus>();
        var definition = new Mock<IFlowDefinition>();
            
        Assert.Throws<ArgumentNullException>(() =>
            new FlowRunnerBaseFake(definition.Object, null, eventBus.Object, GetLogger()));
    }
    
    [Fact]
    public void Ctor_WhenLoggerIsNull_ThrowException()
    {
        var eventBus = new Mock<IEventBus>();   
        var definition = new Mock<IFlowDefinition>();
            
        Assert.Throws<ArgumentNullException>(() =>
            new FlowRunnerBaseFake(definition.Object, Context.Create(eventBus.Object), eventBus.Object, null));
    }
    
    [Fact]
    public void Ctor_WhenDefinitionIsValid_ReturnInstance()
    {
        var eventBus = new Mock<IEventBus>();   
        var definition = new Mock<IFlowDefinition>();
        definition.Setup(mock => mock.InitialNode)
            .Returns(InitialNodeKey);
        definition.Setup(mock => mock.GetNode(InitialNodeKey))
            .Returns(NodeDefinition.Create<NodeFake>(InitialNodeKey));
        
        var instance = new FlowRunnerBaseFake(
            definition.Object, 
            Context.Create(eventBus.Object), 
            eventBus.Object,
            GetLogger());
            
        Assert.NotNull(instance);
        Assert.IsAssignableFrom<IFlowRunner>(instance);
    }
        
    [Fact]
    public void Ctor_WhenInstanceIsCreated_ContextIsAssigned()
    {
        var eventBus = new Mock<IEventBus>();   
        var definition = new Mock<IFlowDefinition>();
        definition.Setup(mock => mock.InitialNode)
            .Returns(InitialNodeKey);
        definition.Setup(mock => mock.GetNode(InitialNodeKey))
            .Returns(NodeDefinition.Create<NodeFake>(InitialNodeKey));
        
        var context = Context.Create(eventBus.Object);
        var instance = new FlowRunnerBaseFake(definition.Object, context, eventBus.Object, GetLogger());
            
        Assert.NotNull(instance.Context);
        Assert.Equal(context, instance.Context);
    }
    #endregion
    
    [Fact]
    public void ExecuteNode_WhenNodeImplementsOnlyINodeBase_ThrowException()
    {
        var eventBus = new Mock<IEventBus>();   
        var definition = new Mock<IFlowDefinition>();
        var context = Context.Create(eventBus.Object);
        
        definition.Setup(mock => mock.InitialNode)
            .Returns(InitialNodeKey);
        definition.Setup(mock => mock.GetNode(InitialNodeKey))
            .Returns(NodeDefinition.Create<NodeFake>(InitialNodeKey));
        
        var instance = new FlowRunnerBaseFake(
            definition.Object, 
            context,
            eventBus.Object,
            GetLogger());

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async ()=> 
            await instance.Start());
    }
        
    [Fact]
    public void ExecuteNode_WhenNodeIsNull_ThrowException()
    {
        var eventBus = new Mock<IEventBus>();
        var definition = new Mock<IFlowDefinition>();
        var context = Context.Create(eventBus.Object);
 
        definition.Setup(mock => mock.InitialNode)
            .Returns(InitialNodeKey);
        definition.Setup(mock => mock.GetNode(InitialNodeKey))
            .Returns(NodeDefinition.Create<NodeFake>(InitialNodeKey));

        var instance = new FlowRunnerBaseFake(
            definition.Object, 
            context,
            eventBus.Object,
            GetLogger());

        Assert.ThrowsAsync<ArgumentNullException>(async ()=> 
            await instance.Start());
    }
}