#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magnett.Automation.Core.Events;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.DocumentMachine;

/// <summary>
/// Document entity that encapsulates a state machine for managing document lifecycle.
/// This demonstrates how to embed a state machine within a domain entity and expose
/// business methods that dispatch actions on the internal state machine.
/// </summary>
public class Document
{
    private readonly IEventBus _eventBus;
    private DocumentStateMachine? _machine;
    
    public string Id { get; }
    public string Title { get; }
    public string Content { get; }
    public DocumentState CurrentState => _machine?.CurrentDocumentState ?? DocumentState.Draft;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public string Author { get; }
    public List<string> Reviewers { get; }
    public string? RejectionReason { get; private set; }

    public Document(string id, string title, string content, string author, IEventBus eventBus)
    {
        Id = id;
        Title = title;
        Content = content;
        Author = author;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Reviewers = new List<string>();
        RejectionReason = null;
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public async Task InitializeAsync()
    {
        _machine = await DocumentStateMachine.CreateAsync(_eventBus);
        UpdatedAt = DateTime.UtcNow;
    }

    public async Task<bool> SubmitAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Document not initialized");
        
        try
        {
            await _machine.DispatchAsync(DocumentAction.Submit);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ApproveAsync(string reviewer)
    {
        if (_machine == null) throw new InvalidOperationException("Document not initialized");
        
        try
        {
            await _machine.DispatchAsync(DocumentAction.Approve);
            AddReviewer(reviewer);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> RejectAsync(string reviewer, string reason)
    {
        if (_machine == null) throw new InvalidOperationException("Document not initialized");
        
        try
        {
            await _machine.DispatchAsync(DocumentAction.Reject);
            RejectionReason = reason;
            AddReviewer(reviewer);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> PublishAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Document not initialized");
        
        try
        {
            await _machine.DispatchAsync(DocumentAction.Publish);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> RequestChangesAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Document not initialized");
        
        try
        {
            await _machine.DispatchAsync(DocumentAction.RequestChanges);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Document not initialized");
        
        try
        {
            await _machine.DispatchAsync(DocumentAction.Delete);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ArchiveAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Document not initialized");
        
        try
        {
            await _machine.DispatchAsync(DocumentAction.Archive);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ReviseAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Document not initialized");
        
        try
        {
            await _machine.DispatchAsync(DocumentAction.Revise);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> RestoreAsync()
    {
        if (_machine == null) throw new InvalidOperationException("Document not initialized");
        
        try
        {
            await _machine.DispatchAsync(DocumentAction.Restore);
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void AddReviewer(string reviewer)
    {
        if (!Reviewers.Contains(reviewer))
        {
            Reviewers.Add(reviewer);
        }
    }
}

