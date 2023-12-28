using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Definitions.Collections;
using Magnett.Automation.Core.WorkFlows.Definitions.Implementations;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;
using Moq;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Definitions.Implementations;

public class FlowDefinitionTest
{
    private const string ExitCode = "OK";
        
    private static readonly CommonNamedKey InitialNodeKey = CommonNamedKey.Create("InitialNode");
    private static readonly CommonNamedKey SecondNodeKey  = CommonNamedKey.Create("SecondNode");
        
    #region Create

    [Fact]
    public void Create_WhenInitialNodeIsNull_ThrowException()
    {
        var nodeList     = new Mock<NodeDefinitionList>();
        var nodeLinkList = new Mock<NodeLinkList>();

        Assert.Throws<ArgumentNullException>(() =>
            FlowDefinition.Create(null, nodeList.Object, nodeLinkList.Object));
    }
    [Fact]
    public void Create_WhenNodeListIsNull_ThrowException()
    {
        var nodeLinkList = new Mock<NodeLinkList>();

        Assert.Throws<ArgumentNullException>(() =>
            FlowDefinition.Create(InitialNodeKey,null, nodeLinkList.Object));
    }       
    [Fact]
    public void Create_WhenNodeLinkListIsNull_ThrowException()
    { 
        var nodeDefinitionList = new Mock<NodeDefinitionList>();

        Assert.Throws<ArgumentNullException>(() =>
            FlowDefinition.Create(InitialNodeKey, nodeDefinitionList.Object, null));       
    }
        
    [Fact]
    public void Create_WhenParametersAreValid_ReturnInstance()
    {
        var nodeDefinitionList = new Mock<NodeDefinitionList>();
        var nodeLinkList = new Mock<NodeLinkList>();

        var instance = FlowDefinition.Create(InitialNodeKey, nodeDefinitionList.Object, nodeLinkList.Object);
            
        Assert.NotNull(instance);
        Assert.IsAssignableFrom<IFlowDefinition>(instance);
    }
    #endregion
        
    [Fact]
    public void InitialNode_WhenInstanceCreated_IsProperStored()
    {
        var nodeDefinitionList = new Mock<NodeDefinitionList>();
        var nodeLinkList = new Mock<NodeLinkList>();

        var instance = FlowDefinition.Create(
            InitialNodeKey, 
            nodeDefinitionList.Object, 
            nodeLinkList.Object);
            
        Assert.Equal(InitialNodeKey, instance.InitialNode);
    }
        
    [Fact]
    public void GetNode_WhenInvoke_CallNodeListGet()
    {
        var nodeDefinitionList = new Mock<NodeDefinitionList>();
        var nodeLinkList = new Mock<NodeLinkList>();

        var definition = FlowDefinition.Create(
            InitialNodeKey, 
            nodeDefinitionList.Object, 
            nodeLinkList.Object);

        _ = definition.GetNode(InitialNodeKey);
            
        nodeDefinitionList.Verify(
            dic =>  dic.Get(InitialNodeKey), 
            Times.Once);
    }
        
    [Fact]
    public void GetNodeByNodeAndCode_WhenLinkExist_ReturnTargetNode()
    {
        var initialNode  = new Mock<INodeDefinition>();
        var secondNode   = new Mock<INodeDefinition>();
        var nodeLink = new Mock<INodeLink>();
        var nodeDefinitionList = new Mock<NodeDefinitionList>();
        var nodeLinkList = new Mock<NodeLinkList>();

        var definition = FlowDefinition.Create(
            InitialNodeKey, 
            nodeDefinitionList.Object, 
            nodeLinkList.Object);

        var nodeLinkKey = NodeLinkKey.Create(InitialNodeKey, ExitCode);
        initialNode.Setup(node => node.Key).Returns(InitialNodeKey);            
        secondNode.Setup(node => node.Key).Returns(SecondNodeKey);
        nodeLink.Setup(link => link.ToNodeKey).Returns(SecondNodeKey);
        nodeLinkList
            .Setup(list => list.HasItem(nodeLinkKey))
            .Returns(true);
        nodeLinkList
            .Setup(list => list.Get(nodeLinkKey))
            .Returns(nodeLink.Object);
        nodeDefinitionList
            .Setup(list => list.Get(SecondNodeKey))
            .Returns(secondNode.Object);

        var node = definition.GetNode(initialNode.Object, ExitCode);
            
        Assert.NotNull(node);
        Assert.Equal(SecondNodeKey, node.Key);
    }

    [Fact]
    public void HasNode_WhenInvoke_CallNodeListHasItem()
    {
        var nodeDefinitionList = new Mock<NodeDefinitionList>();
        var nodeLinkList = new Mock<NodeLinkList>();

        var definition = FlowDefinition.Create(
            InitialNodeKey, 
            nodeDefinitionList.Object, 
            nodeLinkList.Object);

        _ = definition.HasNode(InitialNodeKey);
            
        nodeDefinitionList.Verify(
            dic =>  dic.HasItem(InitialNodeKey), 
            Times.Once);
    }
        
    [Fact]
    public void HasLink_WhenInvoke_CallNodeLinkListHasItem()
    {
        var initialNode = new Mock<INodeDefinition>();
        var nodeDefinitionList = new Mock<NodeDefinitionList>();
        var nodeLinkList = new Mock<NodeLinkList>();
            
        initialNode.Setup(node => node.Key)
            .Returns(InitialNodeKey);

        var definition = FlowDefinition.Create(
            InitialNodeKey, 
            nodeDefinitionList.Object, 
            nodeLinkList.Object);

        _ = definition.HasLink(initialNode.Object, ExitCode);

        var nodeLinkKey = NodeLinkKey.Create(InitialNodeKey, ExitCode);
            
        nodeLinkList.Verify(
            dic =>  dic.HasItem(nodeLinkKey), 
            Times.Once);
    }
}