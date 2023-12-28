using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;
using Magnett.Automation.Core.WorkFlows.Definitions.Implementations;
using Xunit;

using Magnett.Automation.Core.WorkFlows.Runtimes.Factories;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Runtimes.Factories;

public class NodeFactoryTest
{
    [Fact]
    public void Ctor_WhenNodeDefinitionIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            NodeFactory.CreateNode(null));
    }
    
    [Fact]
    public void Ctor_WhenNodeTypeDoesNotHaveValidConstructor_ThrowsInvalidOperationException()
    {
        var nodeDefinition = NodeDefinition
            .Create<InvalidNodeFake>(CommonNamedKey.Create("test"));

        Assert.Throws<InvalidOperationException>(() => 
            NodeFactory.CreateNode(nodeDefinition));
    }
}