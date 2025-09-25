using System;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;
using Moq;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Runtimes.Implementations;

public class FlowTest
{
    #region Completed
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
       
    [Fact]
    public void Run_WhenGetContext_FlowRunnerGetContextIsCalled()
    {
        var eventBus = new Mock<IEventBus>();   
        var context = Context.Create(eventBus.Object);
        var flowRunner = new Mock<IFlowRunner>();

        flowRunner.SetupGet(mock => mock.Context)
            .Returns(context);

        _ = Flow.Create(flowRunner.Object).Context;
            
        flowRunner.VerifyGet(mock => mock.Context, Times.Once);
    }
        
    [Fact]
    public async Task Run_WhenInvoke_FlowRunnerStartIsCalled()
    {
        var flowRunner = new Mock<IFlowRunner>();

        flowRunner.Setup(mock => mock.Start(It.IsAny<CancellationToken>()))
            .ReturnsAsync(NodeExit.Completed("Ok"));

        var flow = Flow.Create(flowRunner.Object);
            
        _ = await flow.Run();
            
        flowRunner.Verify(mock => mock.Start(It.IsAny<CancellationToken>()), Times.Once);
    }
        
    [Fact]
    public async Task Run_WhenInvoke_ReturnsFlowRunnerStart()
    {
        const string code = "ok";
        var flowRunner = new Mock<IFlowRunner>();

        flowRunner.Setup(mock => mock.Start(It.IsAny<CancellationToken>()))
            .ReturnsAsync(NodeExit.Completed(code));

        var flow =  Flow.Create(flowRunner.Object);
        var response = await flow.Run();

        Assert.NotNull(response);
        Assert.Equal(code, response.Code);
    }       
}