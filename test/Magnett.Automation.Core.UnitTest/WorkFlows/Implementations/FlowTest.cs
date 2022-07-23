using System;
using System.Threading.Tasks;
using Magnett.Automation.Core.Contexts;
using Moq;
using Xunit;

using Magnett.Automation.Core.WorkFlows;
using Magnett.Automation.Core.WorkFlows.Implementations;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Implementations;

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
       
    [Fact]
    public void Run_WhenGetContext_FlowRunnerGetContextIsCalled()
    {
        var context = Context.Create();
        var flowRunner = new Mock<IFlowRunner>();

        flowRunner.SetupGet(mock => mock.FlowContext)
            .Returns(context);

        _ = Flow.Create(flowRunner.Object).Context;
            
        flowRunner.VerifyGet(mock => mock.FlowContext, Times.Once);
    }
        
    [Fact]
    public async Task Run_WhenInvoke_FlowRunnerStartIsCalled()
    {
        var flowRunner = new Mock<IFlowRunner>();

        flowRunner.Setup(mock => mock.Start())
            .ReturnsAsync(NodeExit.Create("Ok"));

        var flow = Flow.Create(flowRunner.Object);
            
        _ = await flow.Run();
            
        flowRunner.Verify(mock => mock.Start(), Times.Once);
    }
        
        
    [Fact]
    public async Task Run_WhenInvoke_ReturnsFlowRunnerStart()
    {
        const string code = "ok";
            
        var flowRunner = new Mock<IFlowRunner>();

        flowRunner.Setup(mock => mock.Start())
            .ReturnsAsync(NodeExit.Create(code));

        var flow =  Flow.Create(flowRunner.Object);

        var response = await flow.Run();

        Assert.NotNull(response);
        Assert.Equal(code, response.Code);
    }       
}