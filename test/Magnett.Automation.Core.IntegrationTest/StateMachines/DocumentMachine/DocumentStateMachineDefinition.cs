using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Builders;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.DocumentMachine;

/// <summary>
/// State machine definition for document processing workflow.
/// This demonstrates how to define a complete state machine with all possible transitions.
/// </summary>
public static class DocumentStateMachineDefinition
{
    public static IMachineDefinition Definition { get; } = MachineDefinitionBuilder.Create()
        .InitialState(DocumentState.Draft)
            .OnAction(DocumentAction.Submit).ToState(DocumentState.UnderReview)
            .OnAction(DocumentAction.Delete).ToState(DocumentState.Deleted)
            .Build()
        .AddState(DocumentState.UnderReview)
            .OnAction(DocumentAction.Approve).ToState(DocumentState.Approved)
            .OnAction(DocumentAction.Reject).ToState(DocumentState.Rejected)
            .OnAction(DocumentAction.RequestChanges).ToState(DocumentState.Draft)
            .Build()
        .AddState(DocumentState.Approved)
            .OnAction(DocumentAction.Publish).ToState(DocumentState.Published)
            .OnAction(DocumentAction.Archive).ToState(DocumentState.Archived)
            .Build()
        .AddState(DocumentState.Rejected)
            .OnAction(DocumentAction.Revise).ToState(DocumentState.Draft)
            .OnAction(DocumentAction.Delete).ToState(DocumentState.Deleted)
            .Build()
        .AddState(DocumentState.Published)
            .OnAction(DocumentAction.Update).ToState(DocumentState.Draft)
            .OnAction(DocumentAction.Archive).ToState(DocumentState.Archived)
            .Build()
        .AddState(DocumentState.Archived)
            .OnAction(DocumentAction.Restore).ToState(DocumentState.Draft)
            .Build()
        .AddState(DocumentState.Deleted)
            .Build()
        .BuildDefinition();
}

