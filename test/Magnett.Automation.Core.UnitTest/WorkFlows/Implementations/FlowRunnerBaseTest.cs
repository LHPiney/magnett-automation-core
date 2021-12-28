using System;
using Moq;
using Xunit;

using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.WorkFlows;
using Magnett.Automation.Core.UnitTest.WorkFlows.Implementations.MockUps;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Implementations
{
    public class FlowRunnerBaseTest
    {
        #region Create
        
        [Fact]
        public void Create_WhenDefinitionIsNull_ThrowException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                FlowRunnerBaseMock.Create(null, Context.Create()));
        }
        
        [Fact]
        public void Create_WhenContextIsNull_ThrowException()
        {
            var definition = new Mock<IFlowDefinition>();
            
            Assert.Throws<ArgumentNullException>(() =>
                FlowRunnerBaseMock.Create(definition.Object, null));
        }
        
        [Fact]
        public void Create_WhenDefinitionIsValid_ReturnInstance()
        {
            var definition = new Mock<IFlowDefinition>();

            var instance = FlowRunnerBaseMock.Create(definition.Object, Context.Create());
            
            Assert.NotNull(instance);
            Assert.IsAssignableFrom<IFlowRunner>(instance);
        }
        
        [Fact]
        public void Create_WhenInstanceIsCreated_ContextIsAssigned()
        {
            var definition = new Mock<IFlowDefinition>();
            var context = Context.Create();
            
            var instance = FlowRunnerBaseMock.Create(definition.Object, context);
            
            Assert.NotNull(instance.FlowContext);
            Assert.Equal(context, instance.FlowContext);
        }
        #endregion
        
        
    }
}