using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;
using Magnett.Automation.Server;
using Magnett.Automation.Server.Repositories;
using Magnett.Automation.Server.Repositories.Implementations;
using Moq;
using Xunit;

namespace Magnett.Automation.Server.UnitTest.Repositories;

public class InMemoryFlowRepositoryTest
{
    private readonly IFlowRepository _repository;
    private readonly IEventBus _eventBus;

    public InMemoryFlowRepositoryTest()
    {
        _repository = InMemoryFlowRepository.Create();
        _eventBus = EventBus.Create(Microsoft.Extensions.Logging.Abstractions.NullLogger<EventBus>.Instance);
    }

    private IFlow CreateTestFlow(Guid? id = null)
    {
        var flowId = id ?? Guid.NewGuid();
        var context = Context.Create(_eventBus);
        var mockFlow = new Mock<IFlow>();
        mockFlow.SetupGet(x => x.Id).Returns(flowId);
        mockFlow.SetupGet(x => x.Context).Returns(context);
        return mockFlow.Object;
    }

    [Fact]
    public async Task SaveAsync_WhenFlowIsNull_ReturnFailure()
    {
        var result = await _repository.SaveAsync(null!, FlowState.Running, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal("Flow cannot be null", result.ErrorMessage);
    }

    [Fact]
    public async Task SaveAsync_WhenFlowIsValid_ReturnSuccess()
    {
        var flow = CreateTestFlow();
        
        var result = await _repository.SaveAsync(flow, FlowState.Running, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public async Task SaveAsync_WhenFlowExists_UpdatesFlow()
    {
        var flow = CreateTestFlow();
        await _repository.SaveAsync(flow, FlowState.Running, CancellationToken.None);
        
        var updatedFlow = CreateTestFlow(flow.Id);
        var result = await _repository.SaveAsync(updatedFlow, FlowState.Paused, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        
        var stateResult = await _repository.GetStateAsync(flow.Id, CancellationToken.None);
        Assert.True(stateResult.IsSuccess);
        Assert.Equal(FlowState.Paused, stateResult.Value);
    }

    [Fact]
    public async Task GetAsync_WhenFlowExists_ReturnFlow()
    {
        var flow = CreateTestFlow();
        await _repository.SaveAsync(flow, FlowState.Running, CancellationToken.None);
        
        var result = await _repository.GetAsync(flow.Id, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(flow.Id, result.Value!.Id);
    }

    [Fact]
    public async Task GetAsync_WhenFlowNotExists_ReturnFailure()
    {
        var nonExistentId = Guid.NewGuid();
        
        var result = await _repository.GetAsync(nonExistentId, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Contains(nonExistentId.ToString(), result.ErrorMessage ?? string.Empty);
    }

    [Fact]
    public async Task GetStateAsync_WhenFlowExists_ReturnState()
    {
        var flow = CreateTestFlow();
        await _repository.SaveAsync(flow, FlowState.Completed, CancellationToken.None);
        
        var result = await _repository.GetStateAsync(flow.Id, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(FlowState.Completed, result.Value);
    }

    [Fact]
    public async Task GetStateAsync_WhenFlowNotExists_ReturnFailure()
    {
        var nonExistentId = Guid.NewGuid();
        
        var result = await _repository.GetStateAsync(nonExistentId, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Contains(nonExistentId.ToString(), result.ErrorMessage ?? string.Empty);
    }

    [Fact]
    public async Task GetByStateAsync_WhenFlowsExist_ReturnFlows()
    {
        var flow1 = CreateTestFlow();
        var flow2 = CreateTestFlow();
        var flow3 = CreateTestFlow();
        
        await _repository.SaveAsync(flow1, FlowState.Running, CancellationToken.None);
        await _repository.SaveAsync(flow2, FlowState.Running, CancellationToken.None);
        await _repository.SaveAsync(flow3, FlowState.Paused, CancellationToken.None);
        
        var result = await _repository.GetByStateAsync(FlowState.Running, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        var flows = result.Value!.ToList();
        Assert.Equal(2, flows.Count);
        Assert.Contains(flow1, flows);
        Assert.Contains(flow2, flows);
        Assert.DoesNotContain(flow3, flows);
    }

    [Fact]
    public async Task GetByStateAsync_WhenNoFlowsInState_ReturnEmptyList()
    {
        var result = await _repository.GetByStateAsync(FlowState.Completed, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public async Task UpdateStateAsync_WhenFlowExists_ReturnSuccess()
    {
        var flow = CreateTestFlow();
        await _repository.SaveAsync(flow, FlowState.Running, CancellationToken.None);
        
        var result = await _repository.UpdateStateAsync(flow.Id, FlowState.Paused, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        
        var stateResult = await _repository.GetStateAsync(flow.Id, CancellationToken.None);
        Assert.Equal(FlowState.Paused, stateResult.Value);
    }

    [Fact]
    public async Task UpdateStateAsync_WhenFlowNotExists_ReturnFailure()
    {
        var nonExistentId = Guid.NewGuid();
        
        var result = await _repository.UpdateStateAsync(nonExistentId, FlowState.Paused, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Contains(nonExistentId.ToString(), result.ErrorMessage ?? string.Empty);
    }

    [Fact]
    public async Task DeleteAsync_WhenFlowExists_ReturnSuccess()
    {
        var flow = CreateTestFlow();
        await _repository.SaveAsync(flow, FlowState.Running, CancellationToken.None);
        
        var result = await _repository.DeleteAsync(flow.Id, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        
        var getResult = await _repository.GetAsync(flow.Id, CancellationToken.None);
        Assert.False(getResult.IsSuccess);
    }

    [Fact]
    public async Task DeleteAsync_WhenFlowNotExists_ReturnFailure()
    {
        var nonExistentId = Guid.NewGuid();
        
        var result = await _repository.DeleteAsync(nonExistentId, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Contains(nonExistentId.ToString(), result.ErrorMessage ?? string.Empty);
    }

    [Fact]
    public async Task ExistsAsync_WhenFlowExists_ReturnTrue()
    {
        var flow = CreateTestFlow();
        await _repository.SaveAsync(flow, FlowState.Running, CancellationToken.None);
        
        var result = await _repository.ExistsAsync(flow.Id, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task ExistsAsync_WhenFlowNotExists_ReturnFalse()
    {
        var nonExistentId = Guid.NewGuid();
        
        var result = await _repository.ExistsAsync(nonExistentId, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.False(result.Value);
    }
}

