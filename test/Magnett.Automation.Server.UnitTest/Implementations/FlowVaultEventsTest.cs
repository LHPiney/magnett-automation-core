using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Magnett.Automation.Core.WorkFlows.Definitions;
using Magnett.Automation.Core.WorkFlows.Definitions.Builders;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Server;
using Magnett.Automation.Server.Events;
using Magnett.Automation.Server.Implementations;
using Magnett.Automation.Server.Repositories;
using Magnett.Automation.Server.Repositories.Implementations;
using Magnett.Automation.Server.UnitTest.Fakes;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Magnett.Automation.Server.UnitTest.Implementations;

public class FlowVaultEventsTest
{
    private readonly IFlowDefinitionRegistry _definitionRegistry;
    private readonly IFlowRepository _repository;
    private readonly IEventBus _eventBus;

    public FlowVaultEventsTest()
    {
        var mockRegistry = new Mock<IFlowDefinitionRegistry>();
        _definitionRegistry = mockRegistry.Object;
        
        _repository = InMemoryFlowRepository.Create();
        _eventBus = EventBus.Create(NullLogger<EventBus>.Instance);
        _eventBus.Start();
    }

    private IFlowDefinition CreateTestDefinition(string key, int version)
    {
        var definition = FlowDefinitionBuilder.Create()
            .WithInitialNode<NodeFake>(CommonNamedKey.Create("Start"))
            .Build()
            .BuildDefinition();
        
        var mockDefinition = new Mock<IFlowDefinition>();
        var definitionKey = CommonNamedKey.Create(key);
        mockDefinition.SetupGet(x => x.Key).Returns(definitionKey);
        mockDefinition.SetupGet(x => x.Version).Returns(version);
        mockDefinition.SetupGet(x => x.InitialNode).Returns(definition.InitialNode);
        mockDefinition.Setup(x => x.HasNode(It.IsAny<CommonNamedKey>())).Returns(definition.HasNode);
        mockDefinition.Setup(x => x.GetNode(It.IsAny<CommonNamedKey>())).Returns((CommonNamedKey k) => definition.GetNode(k));
        mockDefinition.Setup(x => x.HasLink(It.IsAny<CommonNamedKey>(), It.IsAny<string>())).Returns(definition.HasLink);
        mockDefinition.Setup(x => x.GetLink(It.IsAny<CommonNamedKey>(), It.IsAny<string>())).Returns((CommonNamedKey k, string c) => definition.GetLink(k, c));
        return mockDefinition.Object;
    }

    private FlowVault CreateVault()
    {
        return new FlowVault(_definitionRegistry, _repository, _eventBus, NullLogger<FlowVault>.Instance);
    }

    [Fact]
    public async Task PublishDefinitionAsync_WhenSuccessful_EmitsFlowDefinitionPublishedEvent()
    {
        var vault = CreateVault();
        var definition = CreateTestDefinition("test-flow", 1);
        
        var mockRegistry = Mock.Get(_definitionRegistry);
        mockRegistry.Setup(x => x.RegisterAsync(definition, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var result = await vault.PublishDefinitionAsync(definition, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(vault.LocalEvents);
        var emittedEvent = vault.LocalEvents.First() as FlowDefinitionPublishedEvent;
        Assert.NotNull(emittedEvent);
        Assert.Equal("test-flow", emittedEvent.DefinitionKey);
        Assert.Equal(1, emittedEvent.Version);
    }

    [Fact]
    public async Task PublishDefinitionAsync_WhenFails_EmitsFlowDefinitionPublishFailedEvent()
    {
        var vault = CreateVault();
        var definition = CreateTestDefinition("test-flow", 1);
        
        var mockRegistry = Mock.Get(_definitionRegistry);
        mockRegistry.Setup(x => x.RegisterAsync(definition, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure("Registration failed"));

        var result = await vault.PublishDefinitionAsync(definition, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Single(vault.LocalEvents);
        var emittedEvent = vault.LocalEvents.First() as FlowDefinitionPublishFailedEvent;
        Assert.NotNull(emittedEvent);
        Assert.Equal("test-flow", emittedEvent.DefinitionKey);
        Assert.Equal(1, emittedEvent.Version);
        Assert.Equal("Registration failed", emittedEvent.ErrorMessage);
    }

    [Fact]
    public async Task CreateInstanceAsync_WhenSuccessful_EmitsFlowInstanceCreatedEvent()
    {
        var vault = CreateVault();
        var definitionKey = "test-flow";
        var definition = CreateTestDefinition(definitionKey, 1);
        var initialContext = Context.Create(_eventBus);
        
        var mockRegistry = Mock.Get(_definitionRegistry);
        mockRegistry.Setup(x => x.GetAsync(definitionKey, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IFlowDefinition>.Success(definition));

        var result = await vault.CreateInstanceAsync(definitionKey, initialContext, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(vault.LocalEvents);
        var emittedEvent = vault.LocalEvents.First() as FlowInstanceCreatedEvent;
        Assert.NotNull(emittedEvent);
        Assert.Equal(result.Value!.Id, emittedEvent.FlowId);
        Assert.Equal(definitionKey, emittedEvent.DefinitionKey);
        Assert.Equal(1, emittedEvent.DefinitionVersion);
    }

    [Fact]
    public async Task PauseInstanceAsync_WhenSuccessful_EmitsFlowInstancePausedAndStateChangedEvents()
    {
        var vault = CreateVault();
        var definitionKey = "test-flow";
        var definition = CreateTestDefinition(definitionKey, 1);
        var initialContext = Context.Create(_eventBus);
        
        var mockRegistry = Mock.Get(_definitionRegistry);
        mockRegistry.Setup(x => x.GetAsync(definitionKey, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IFlowDefinition>.Success(definition));

        var createResult = await vault.CreateInstanceAsync(definitionKey, initialContext, CancellationToken.None);
        Assert.True(createResult.IsSuccess);
        
        var initialEventCount = vault.LocalEvents.Count;

        var pauseResult = await vault.PauseInstanceAsync(createResult.Value!.Id, CancellationToken.None);

        Assert.True(pauseResult.IsSuccess);
        Assert.Equal(initialEventCount + 2, vault.LocalEvents.Count);
        
        var newEvents = vault.LocalEvents.Skip(initialEventCount).ToList();
        var pausedEvent = newEvents.OfType<FlowInstancePausedEvent>().FirstOrDefault();
        Assert.NotNull(pausedEvent);
        Assert.Equal(createResult.Value.Id, pausedEvent.FlowId);

        var stateChangedEvent = newEvents.OfType<FlowInstanceStateChangedEvent>().FirstOrDefault();
        Assert.NotNull(stateChangedEvent);
        Assert.Equal(createResult.Value.Id, stateChangedEvent.FlowId);
        Assert.Equal("Running", stateChangedEvent.PreviousState);
        Assert.Equal("Paused", stateChangedEvent.NewState);
    }

    [Fact]
    public async Task ResumeInstanceAsync_WhenSuccessful_EmitsFlowInstanceResumedAndStateChangedEvents()
    {
        var vault = CreateVault();
        var definitionKey = "test-flow";
        var definition = CreateTestDefinition(definitionKey, 1);
        var initialContext = Context.Create(_eventBus);
        
        var mockRegistry = Mock.Get(_definitionRegistry);
        mockRegistry.Setup(x => x.GetAsync(definitionKey, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IFlowDefinition>.Success(definition));

        var createResult = await vault.CreateInstanceAsync(definitionKey, initialContext, CancellationToken.None);
        Assert.True(createResult.IsSuccess);
        
        var pauseResult = await vault.PauseInstanceAsync(createResult.Value!.Id, CancellationToken.None);
        Assert.True(pauseResult.IsSuccess);
        
        var pauseStateChangedEvents = vault.LocalEvents.OfType<FlowInstanceStateChangedEvent>().ToList();
        var lastPauseEvent = pauseStateChangedEvents.LastOrDefault();
        Assert.NotNull(lastPauseEvent);
        Assert.Equal("Paused", lastPauseEvent.NewState);
        
        var initialEventCount = vault.LocalEvents.Count;

        var resumeResult = await vault.ResumeInstanceAsync(createResult.Value.Id, CancellationToken.None);

        Assert.True(resumeResult.IsSuccess);
        Assert.Equal(initialEventCount + 2, vault.LocalEvents.Count);
        
        var newEvents = vault.LocalEvents.Skip(initialEventCount).ToList();
        var resumedEvent = newEvents.OfType<FlowInstanceResumedEvent>().FirstOrDefault();
        Assert.NotNull(resumedEvent);
        Assert.Equal(createResult.Value.Id, resumedEvent.FlowId);

        var stateChangedEvents = newEvents.OfType<FlowInstanceStateChangedEvent>().ToList();
        Assert.Single(stateChangedEvents);
        var stateChangedEvent = stateChangedEvents.First();
        Assert.Equal(createResult.Value.Id, stateChangedEvent.FlowId);
        Assert.Equal("Paused", stateChangedEvent.PreviousState);
        Assert.Equal("Running", stateChangedEvent.NewState);
    }

    [Fact]
    public async Task PauseInstanceAsync_WhenAlreadyPaused_DoesNotEmitEvents()
    {
        var vault = CreateVault();
        var definitionKey = "test-flow";
        var definition = CreateTestDefinition(definitionKey, 1);
        var initialContext = Context.Create(_eventBus);
        
        var mockRegistry = Mock.Get(_definitionRegistry);
        mockRegistry.Setup(x => x.GetAsync(definitionKey, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IFlowDefinition>.Success(definition));

        var createResult = await vault.CreateInstanceAsync(definitionKey, initialContext, CancellationToken.None);
        Assert.True(createResult.IsSuccess);
        
        var pauseResult1 = await vault.PauseInstanceAsync(createResult.Value!.Id, CancellationToken.None);
        Assert.True(pauseResult1.IsSuccess);
        
        var initialEventCount = vault.LocalEvents.Count;

        var pauseResult2 = await vault.PauseInstanceAsync(createResult.Value.Id, CancellationToken.None);
        Assert.True(pauseResult2.IsSuccess);
        
        Assert.Equal(initialEventCount, vault.LocalEvents.Count);
    }

    [Fact]
    public async Task CreateInstanceAsync_WhenFails_EmitsFlowInstanceCreationFailedEvent()
    {
        var vault = CreateVault();
        var definitionKey = "non-existent";
        
        var mockRegistry = Mock.Get(_definitionRegistry);
        mockRegistry.Setup(x => x.GetAsync(definitionKey, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IFlowDefinition>.Failure("Definition not found"));

        var initialContext = Context.Create(_eventBus);
        var result = await vault.CreateInstanceAsync(definitionKey, initialContext, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Single(vault.LocalEvents);
        var emittedEvent = vault.LocalEvents.First() as FlowInstanceCreationFailedEvent;
        Assert.NotNull(emittedEvent);
        Assert.Equal(definitionKey, emittedEvent.DefinitionKey);
        Assert.Equal("Definition not found", emittedEvent.ErrorMessage);
    }

    [Fact]
    public async Task CreateInstanceAsync_WhenSaveFails_EmitsFlowInstanceCreationFailedEvent()
    {
        var vault = CreateVault();
        var definitionKey = "test-flow";
        var definition = CreateTestDefinition(definitionKey, 1);
        var initialContext = Context.Create(_eventBus);
        
        var mockRegistry = Mock.Get(_definitionRegistry);
        mockRegistry.Setup(x => x.GetAsync(definitionKey, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IFlowDefinition>.Success(definition));

        var mockRepository = new Mock<IFlowRepository>();
        mockRepository.Setup(x => x.SaveAsync(It.IsAny<IFlow>(), It.IsAny<FlowState>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure("Save failed"));

        var vaultWithMockRepo = new FlowVault(_definitionRegistry, mockRepository.Object, _eventBus, NullLogger<FlowVault>.Instance);

        var result = await vaultWithMockRepo.CreateInstanceAsync(definitionKey, initialContext, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Single(vaultWithMockRepo.LocalEvents);
        var emittedEvent = vaultWithMockRepo.LocalEvents.First() as FlowInstanceCreationFailedEvent;
        Assert.NotNull(emittedEvent);
        Assert.Equal(definitionKey, emittedEvent.DefinitionKey);
        Assert.Equal("Save failed", emittedEvent.ErrorMessage);
    }

    [Fact]
    public async Task GetDefinitionAsync_WhenSuccessful_EmitsFlowDefinitionRetrievedEvent()
    {
        var vault = CreateVault();
        var definitionKey = "test-flow";
        var definition = CreateTestDefinition(definitionKey, 1);
        
        var mockRegistry = Mock.Get(_definitionRegistry);
        mockRegistry.Setup(x => x.GetAsync(definitionKey, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IFlowDefinition>.Success(definition));

        var result = await vault.GetDefinitionAsync(definitionKey, null, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(vault.LocalEvents);
        var emittedEvent = vault.LocalEvents.First() as FlowDefinitionRetrievedEvent;
        Assert.NotNull(emittedEvent);
        Assert.Equal(definitionKey, emittedEvent.DefinitionKey);
        Assert.Null(emittedEvent.Version);
    }

    [Fact]
    public async Task GetDefinitionAsync_WhenSuccessfulWithVersion_EmitsFlowDefinitionRetrievedEventWithVersion()
    {
        var vault = CreateVault();
        var definitionKey = "test-flow";
        var version = 2;
        var definition = CreateTestDefinition(definitionKey, version);
        
        var mockRegistry = Mock.Get(_definitionRegistry);
        mockRegistry.Setup(x => x.GetAsync(definitionKey, version, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IFlowDefinition>.Success(definition));

        var result = await vault.GetDefinitionAsync(definitionKey, version, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(vault.LocalEvents);
        var emittedEvent = vault.LocalEvents.First() as FlowDefinitionRetrievedEvent;
        Assert.NotNull(emittedEvent);
        Assert.Equal(definitionKey, emittedEvent.DefinitionKey);
        Assert.Equal(version, emittedEvent.Version);
    }

    [Fact]
    public async Task GetDefinitionAsync_WhenFails_DoesNotEmitFlowDefinitionRetrievedEvent()
    {
        var vault = CreateVault();
        var definitionKey = "non-existent";
        
        var mockRegistry = Mock.Get(_definitionRegistry);
        mockRegistry.Setup(x => x.GetAsync(definitionKey, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IFlowDefinition>.Failure("Definition not found"));

        var result = await vault.GetDefinitionAsync(definitionKey, null, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Empty(vault.LocalEvents);
    }

    [Fact]
    public async Task GetInstanceAsync_WhenSuccessful_EmitsFlowInstanceRetrievedEvent()
    {
        var vault = CreateVault();
        var definitionKey = "test-flow";
        var definition = CreateTestDefinition(definitionKey, 1);
        var initialContext = Context.Create(_eventBus);
        
        var mockRegistry = Mock.Get(_definitionRegistry);
        mockRegistry.Setup(x => x.GetAsync(definitionKey, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IFlowDefinition>.Success(definition));

        var createResult = await vault.CreateInstanceAsync(definitionKey, initialContext, CancellationToken.None);
        Assert.True(createResult.IsSuccess);
        
        var initialEventCount = vault.LocalEvents.Count;

        var getResult = await vault.GetInstanceAsync(createResult.Value!.Id, CancellationToken.None);

        Assert.True(getResult.IsSuccess);
        Assert.Equal(initialEventCount + 1, vault.LocalEvents.Count);
        var emittedEvent = vault.LocalEvents.Skip(initialEventCount).First() as FlowInstanceRetrievedEvent;
        Assert.NotNull(emittedEvent);
        Assert.Equal(createResult.Value.Id, emittedEvent.FlowId);
    }

    [Fact]
    public async Task GetInstanceAsync_WhenFails_DoesNotEmitFlowInstanceRetrievedEvent()
    {
        var vault = CreateVault();
        var nonExistentId = Guid.NewGuid();
        
        var result = await vault.GetInstanceAsync(nonExistentId, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Empty(vault.LocalEvents);
    }

    [Fact]
    public async Task SaveInstanceAsync_WhenSuccessful_EmitsFlowInstanceSavedEvent()
    {
        var vault = CreateVault();
        var definitionKey = "test-flow";
        var definition = CreateTestDefinition(definitionKey, 1);
        var initialContext = Context.Create(_eventBus);
        
        var mockRegistry = Mock.Get(_definitionRegistry);
        mockRegistry.Setup(x => x.GetAsync(definitionKey, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IFlowDefinition>.Success(definition));

        var createResult = await vault.CreateInstanceAsync(definitionKey, initialContext, CancellationToken.None);
        Assert.True(createResult.IsSuccess);
        
        var initialEventCount = vault.LocalEvents.Count;

        var saveResult = await vault.SaveInstanceAsync(createResult.Value!, CancellationToken.None);

        Assert.True(saveResult.IsSuccess);
        Assert.Equal(initialEventCount + 1, vault.LocalEvents.Count);
        
        var savedEvent = vault.LocalEvents.Skip(initialEventCount).First() as FlowInstanceSavedEvent;
        Assert.NotNull(savedEvent);
        Assert.Equal(createResult.Value.Id, savedEvent.FlowId);
        Assert.Equal("Running", savedEvent.State);
    }

    [Fact]
    public async Task SaveInstanceAsync_WhenSaveFails_EmitsFlowInstanceSaveFailedEvent()
    {
        var vault = CreateVault();
        var definitionKey = "test-flow";
        var definition = CreateTestDefinition(definitionKey, 1);
        var initialContext = Context.Create(_eventBus);
        
        var mockRegistry = Mock.Get(_definitionRegistry);
        mockRegistry.Setup(x => x.GetAsync(definitionKey, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IFlowDefinition>.Success(definition));

        var createResult = await vault.CreateInstanceAsync(definitionKey, initialContext, CancellationToken.None);
        Assert.True(createResult.IsSuccess);
        
        var mockRepository = new Mock<IFlowRepository>();
        mockRepository.Setup(x => x.ExistsAsync(createResult.Value!.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));
        mockRepository.Setup(x => x.GetStateAsync(createResult.Value.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<FlowState>.Success(FlowState.Running));
        mockRepository.Setup(x => x.SaveAsync(createResult.Value, It.IsAny<FlowState>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure("Save failed"));

        var vaultWithMockRepo = new FlowVault(_definitionRegistry, mockRepository.Object, _eventBus, NullLogger<FlowVault>.Instance);
        
        var initialEventCount = vaultWithMockRepo.LocalEvents.Count;

        var saveResult = await vaultWithMockRepo.SaveInstanceAsync(createResult.Value, CancellationToken.None);

        Assert.True(saveResult.IsFailure);
        Assert.Equal(initialEventCount + 1, vaultWithMockRepo.LocalEvents.Count);
        
        var failedEvent = vaultWithMockRepo.LocalEvents.Skip(initialEventCount).First() as FlowInstanceSaveFailedEvent;
        Assert.NotNull(failedEvent);
        Assert.Equal(createResult.Value.Id, failedEvent.FlowId);
        Assert.Equal("Save failed", failedEvent.ErrorMessage);
    }

    [Fact]
    public async Task SaveInstanceAsync_WhenGetStateFails_EmitsFlowInstanceSaveFailedEvent()
    {
        var vault = CreateVault();
        var definitionKey = "test-flow";
        var definition = CreateTestDefinition(definitionKey, 1);
        var initialContext = Context.Create(_eventBus);
        
        var mockRegistry = Mock.Get(_definitionRegistry);
        mockRegistry.Setup(x => x.GetAsync(definitionKey, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IFlowDefinition>.Success(definition));

        var createResult = await vault.CreateInstanceAsync(definitionKey, initialContext, CancellationToken.None);
        Assert.True(createResult.IsSuccess);
        
        var mockRepository = new Mock<IFlowRepository>();
        mockRepository.Setup(x => x.ExistsAsync(createResult.Value!.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));
        mockRepository.Setup(x => x.GetStateAsync(createResult.Value.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<FlowState>.Failure("Get state failed"));

        var vaultWithMockRepo = new FlowVault(_definitionRegistry, mockRepository.Object, _eventBus, NullLogger<FlowVault>.Instance);
        
        var initialEventCount = vaultWithMockRepo.LocalEvents.Count;

        var saveResult = await vaultWithMockRepo.SaveInstanceAsync(createResult.Value, CancellationToken.None);

        Assert.True(saveResult.IsFailure);
        Assert.Equal(initialEventCount + 1, vaultWithMockRepo.LocalEvents.Count);
        
        var failedEvent = vaultWithMockRepo.LocalEvents.Skip(initialEventCount).First() as FlowInstanceSaveFailedEvent;
        Assert.NotNull(failedEvent);
        Assert.Equal(createResult.Value.Id, failedEvent.FlowId);
        Assert.Equal("Get state failed", failedEvent.ErrorMessage);
    }
}
