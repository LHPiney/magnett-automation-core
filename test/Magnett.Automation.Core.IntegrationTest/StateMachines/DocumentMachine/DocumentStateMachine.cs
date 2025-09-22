using System.Threading.Tasks;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.StateMachines;
using Magnett.Automation.Core.StateMachines.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.DocumentMachine;

/// <summary>
/// Document state machine implementation that inherits from Machine.
/// This demonstrates the recommended pattern for creating state machines.
/// </summary>
public sealed class DocumentStateMachine : Machine
{
    private DocumentStateMachine(IMachineDefinition definition, IEventBus eventBus) : base(definition, eventBus)
    {
        
    }

    public static async Task<DocumentStateMachine> CreateAsync(IEventBus eventBus)
    {
        var machine = new DocumentStateMachine(DocumentStateMachineDefinition.Definition, eventBus);
        await machine.InitializeAsync();
        return machine;
    }

    /// <summary>
    /// Gets the current state as a DocumentState enumeration.
    /// This provides a type-safe way to access the current state.
    /// </summary>
    public DocumentState CurrentDocumentState => this switch
    {
        var machine when machine.Equals(DocumentState.Draft) => DocumentState.Draft,
        var machine when machine.Equals(DocumentState.UnderReview) => DocumentState.UnderReview,
        var machine when machine.Equals(DocumentState.Approved) => DocumentState.Approved,
        var machine when machine.Equals(DocumentState.Rejected) => DocumentState.Rejected,
        var machine when machine.Equals(DocumentState.Published) => DocumentState.Published,
        var machine when machine.Equals(DocumentState.Archived) => DocumentState.Archived,
        var machine when machine.Equals(DocumentState.Deleted) => DocumentState.Deleted,
        _ => DocumentState.Draft
    };
}

