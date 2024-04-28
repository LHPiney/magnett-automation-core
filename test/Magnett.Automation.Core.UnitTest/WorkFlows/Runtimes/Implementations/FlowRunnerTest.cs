using System;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Definitions.Implementations;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;
using Moq;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Runtimes.Implementations;

public class FlowRunnerTest
{
    private static readonly CommonNamedKey InitialNodeKey = CommonNamedKey.Create("InitialNode");
    private static readonly CommonNamedKey SecondNodeKey  = CommonNamedKey.Create("SecondNode");

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
        definition.Setup(mock => mock.InitialNode)
            .Returns(InitialNodeKey);
        definition.Setup(mock => mock.GetNode(InitialNodeKey))
            .Returns(NodeDefinition.Create<NodeFake>(InitialNodeKey));
            
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
        
        definition.Setup(mock => mock.InitialNode)
            .Returns(InitialNodeKey);
        definition.Setup(mock => mock.GetNode(InitialNodeKey))
            .Returns(NodeDefinition.Create<NodeFake>(InitialNodeKey));

        node
            .Setup(def => def.Execute(It.IsNotNull<Context>()))
            .ReturnsAsync(NodeExit.Create("node"));
            
        definition
            .SetupGet(def => def.InitialNode)
            .Returns(InitialNodeKey);

        var instance = FlowRunner.Create(
            definition.Object, 
            context);

        var nodeExit = await instance.Start();
    }
}