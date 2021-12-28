using System;
using Moq;
using Xunit;

using Magnett.Automation.Core.WorkFlows;
using Magnett.Automation.Core.WorkFlows.Implementations;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Implementations
{
    public class FlowTest
    {
        #region Create
        [Fact]
        public void Create_WhenInvoke_WhenFlowRunnerIsNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                Flow.Create(null));
        }
        
        [Fact]
        public void Create_WhenInvoke_WhenFlowRunnerIsValid_ReturnInstance()
        {
            var flowRunner = new Mock<IFlowRunner>();

            var instance = Flow.Create(flowRunner.Object);
            
            Assert.NotNull(instance);
            Assert.IsAssignableFrom<IFlow>(instance);
        }

        [Fact]
        public void Create_WhenInvoke_NewIdIsAssigned()
        {
            var flowRunner = new Mock<IFlowRunner>();

            var instance = Flow.Create(flowRunner.Object);

            Assert.NotEqual(Guid.Empty, instance.Id);
        }
        #endregion

        
    }
}