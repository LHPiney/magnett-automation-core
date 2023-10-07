using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;
using Magnett.Automation.Core.WorkFlows;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Runtimes;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Implementations;

public class FlowRunnerBaseTest
{
    #region Ctor
        
    [Fact]
    public void Ctor_WhenDefinitionIsNull_ThrowException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new FlowRunnerBaseFake(null, Context.Create()));
    }
        
    [Fact]
    public void Ctor_WhenContextIsNull_ThrowException()
    {
        var definition = new Mock<IFlowDefinition>();
            
        Assert.Throws<ArgumentNullException>(() =>
            new FlowRunnerBaseFake(definition.Object, null));
    }
        
    [Fact]
    public void Ctor_WhenDefinitionIsValid_ReturnInstance()
    {
        var definition = new Mock<IFlowDefinition>();

        var instance = new FlowRunnerBaseFake(definition.Object, Context.Create());
            
        Assert.NotNull(instance);
        Assert.IsAssignableFrom<IFlowRunner>(instance);
    }
        
    [Fact]
    public void Ctor_WhenInstanceIsCreated_ContextIsAssigned()
    {
        var definition = new Mock<IFlowDefinition>();
        var context = Context.Create();
            
        var instance = new FlowRunnerBaseFake(definition.Object, context);
            
        Assert.NotNull(instance.FlowContext);
        Assert.Equal(context, instance.FlowContext);
    }
    #endregion

    [Fact]
    public async Task ExecuteNode_WhenNodeIsAsync_CallExecuteNodeAsync()
    {
        var node = new Mock<INodeAsync>();
        var definition = new Mock<IFlowDefinition>();
        var context = Context.Create();

        node
            .Setup(def => def.Execute(It.IsNotNull<Context>()))
            .ReturnsAsync(NodeExit.Create("node"));
            
        definition
            .SetupGet(def => def.InitialNode)
            .Returns(node.Object);

        var instance = new FlowRunnerBaseFake(
            definition.Object, 
            context);

        var nodeExit = await instance.Start();
            
        Assert.NotNull(nodeExit);
        node.Verify(moq => 
            moq.Execute(It.IsNotNull<Context>()), Times.Once);
    }
        
    [Fact]
    public async Task ExecuteNode_WhenNodeIsSync_CallExecuteNodeSync()
    {
        var node = new Mock<INode>();
        var definition = new Mock<IFlowDefinition>();
        var context = Context.Create();

        node
            .Setup(def => 
                def.Execute(It.IsNotNull<Context>()))
            .Returns(NodeExit.Create("node"));
            
        definition
            .SetupGet(def => def.InitialNode)
            .Returns(node.Object);

        var instance = new FlowRunnerBaseFake(
            definition.Object, 
            context);

        var nodeExit = await instance.Start();
            
        Assert.NotNull(nodeExit);
        node.Verify(moq => 
            moq.Execute(It.IsNotNull<Context>()), Times.Once);
    }
        
    [Fact]
    public void ExecuteNode_WhenNodeImplementsOnlyINodeBase_ThrowException()
    {
        var node = new Mock<INodeBase>();
        var definition = new Mock<IFlowDefinition>();
        var context = Context.Create();
            
        definition
            .SetupGet(def => def.InitialNode)
            .Returns(node.Object);

        var instance = new FlowRunnerBaseFake(
            definition.Object, 
            context);

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async ()=> 
            await instance.Start());
    }
        
    [Fact]
    public void ExecuteNode_WhenNodeIsNull_ThrowException()
    {
        var definition = new Mock<IFlowDefinition>();
        var context = Context.Create();
            
        definition
            .SetupGet(def => def.InitialNode)
            .Returns((INodeBase)null);

        var instance = new FlowRunnerBaseFake(
            definition.Object, 
            context);

        Assert.ThrowsAsync<ArgumentNullException>(async ()=> 
            await instance.Start());
    }
}