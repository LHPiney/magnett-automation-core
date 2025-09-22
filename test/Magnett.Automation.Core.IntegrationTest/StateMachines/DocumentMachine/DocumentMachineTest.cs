using Magnett.Automation.Core.IntegrationTest.StateMachines.DocumentMachine;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.DocumentMachine;

/// <summary>
/// Integration test for Document Processing State Machine example.
/// This test demonstrates the complete workflow of document processing
/// and validates that all state transitions work correctly.
/// </summary>
public class DocumentMachineTest
{
    [Fact]
    public void CreateDefinition_GetValidDefinition()
    {
        var definition = DocumentStateMachineDefinition.Definition;
            
        Assert.NotNull(definition);
        Assert.NotNull(definition.InitialState);
        Assert.Equal("Draft", definition.InitialState.Key.Name);
    }

    [Fact]
    public async Task Document_Should_Transition_Through_Complete_Workflow()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create a document
            var document = new Document("doc-1", "Sample Document", "This is a sample document", "author-1", eventBus);
            Assert.Equal(DocumentState.Draft, document.CurrentState);

            // Initialize the document with its state machine
            await document.InitializeAsync();
            Assert.Equal(DocumentState.Draft, document.CurrentState);

            // Submit document for review
            var submitted = await document.SubmitAsync();
            Assert.True(submitted);
            Assert.Equal(DocumentState.UnderReview, document.CurrentState);

            // Approve document
            var approved = await document.ApproveAsync("reviewer-1");
            Assert.True(approved);
            Assert.Equal(DocumentState.Approved, document.CurrentState);
            Assert.Contains("reviewer-1", document.Reviewers);

            // Publish document
            var published = await document.PublishAsync();
            Assert.True(published);
            Assert.Equal(DocumentState.Published, document.CurrentState);
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task Document_Should_Handle_Rejection_Workflow()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create and initialize document
            var document = new Document("doc-2", "Rejected Document", "This document will be rejected", "author-2", eventBus);
            await document.InitializeAsync();

            // Submit document
            await document.SubmitAsync();
            Assert.Equal(DocumentState.UnderReview, document.CurrentState);

            // Reject document
            var rejected = await document.RejectAsync("reviewer-2", "Needs more detail");
            Assert.True(rejected);
            Assert.Equal(DocumentState.Rejected, document.CurrentState);
            Assert.Equal("Needs more detail", document.RejectionReason);
            Assert.Contains("reviewer-2", document.Reviewers);

            // Revise document (goes back to Draft) - from Rejected state, we need to use Revise action
            // Note: RequestChanges is only available from UnderReview state
            // From Rejected state, we can only go to Draft with Revise action
            var revised = await document.ReviseAsync();
            Assert.True(revised);
            Assert.Equal(DocumentState.Draft, document.CurrentState);
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task Document_Should_Handle_Archive_And_Restore_Workflow()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create and initialize document
            var document = new Document("doc-3", "Archived Document", "This document will be archived", "author-3", eventBus);
            await document.InitializeAsync();

            // Submit and approve document
            await document.SubmitAsync();
            await document.ApproveAsync("reviewer-3");
            await document.PublishAsync();
            Assert.Equal(DocumentState.Published, document.CurrentState);

            // Archive document
            var archived = await document.ArchiveAsync();
            Assert.True(archived);
            Assert.Equal(DocumentState.Archived, document.CurrentState);

            // Restore document (goes back to Draft)
            var restored = await document.RestoreAsync();
            Assert.True(restored);
            Assert.Equal(DocumentState.Draft, document.CurrentState);
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task Document_Should_Handle_Deletion_Workflow()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create and initialize document
            var document = new Document("doc-4", "Deleted Document", "This document will be deleted", "author-4", eventBus);
            await document.InitializeAsync();

            // Delete document from Draft state
            var deleted = await document.DeleteAsync();
            Assert.True(deleted);
            Assert.Equal(DocumentState.Deleted, document.CurrentState);
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task Document_Should_Throw_Exception_When_Not_Initialized()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create document but don't initialize
            var document = new Document("doc-5", "Uninitialized Document", "This document is not initialized", "author-5", eventBus);

            // Should throw exception when trying to submit without initialization
            await Assert.ThrowsAsync<InvalidOperationException>(() => document.SubmitAsync());
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }

    [Fact]
    public async Task Document_Should_Handle_Invalid_Transitions_Gracefully()
    {
        // Setup logging and event bus
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var eventBus = EventBus.Create(loggerFactory.CreateLogger<EventBus>());
        eventBus.Start();

        try
        {
            // Create and initialize document
            var document = new Document("doc-6", "Test Document", "Testing invalid transitions", "author-6", eventBus);
            await document.InitializeAsync();

            // Try to approve from Draft state (should fail)
            var approved = await document.ApproveAsync("reviewer-6");
            Assert.False(approved); // Should return false, not throw exception
            Assert.Equal(DocumentState.Draft, document.CurrentState); // Should remain in Draft
        }
        finally
        {
            await eventBus.StopAsync();
        }
    }
}
