using System;
using Magnett.Automation.Core.Contexts;
using Moq;
using Xunit;

using Magnett.Automation.Core.WorkFlows;
using Magnett.Automation.Core.WorkFlows.Implementations;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Implementations
{
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
        #endregion
        
        [Fact]
        public void Create_WhenDefinitionIsValid_ReturnInstance()
        {
            var definition = new Mock<IFlowDefinition>();

            var instance = FlowRunner.Create(definition.Object, Context.Create());
            
            Assert.NotNull(instance);
            Assert.IsAssignableFrom<IFlowRunner>(instance);
        }
    }
}