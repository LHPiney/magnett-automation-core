using System;
using System.Threading.Tasks;
using Magnett.Automation.Core.Contexts;
using Moq;
using Xunit;

using Magnett.Automation.Core.WorkFlows;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Implementations;

public class FlowRunnerTest
{
    #region Create
    [Fact]
    public void Create_WhenDefinitionIsNull_ThrowException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            FlowRunner.Create(null, Context.Create()));
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
        var definition = new Mock<IFlowDefinition>();

        var instance = FlowRunner.Create(definition.Object, Context.Create());
            
        Assert.NotNull(instance);
        Assert.IsAssignableFrom<IFlowRunner>(instance);
    }
    #endregion

    [Fact]
    public async Task Start_WhenNodeToRunNotIsNull_CallNodeExecute()
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

        var instance = FlowRunner.Create(
            definition.Object, 
            context);

        var nodeExit = await instance.Start();
    }
}