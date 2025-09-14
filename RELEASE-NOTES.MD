v.0.5.1
- Initial serious release

v.0.6.0
- Split Runtime from Definition in Workflow declaration
- Add support for net Core 8.

v.1.0.0
- Definition of Workflow does not require an instance, just using generic type
- Total separation between Definition and Runtime
- Minor changes in runtime to increase performance
- Add support for net Core 9.0

v.1.0.1 (2025-09-14)
- WorkFlows.Runtimes.NodeBase: BUGFIX — failure path now emits `OnNodeFailedEvent` instead of `OnNodeCompletedEvent`.
- Migration: If you were listening for `OnNodeCompletedEvent` when a node failed, update subscribers to handle `OnNodeFailedEvent` instead. Update any tests asserting the emitted event on failure.

- Events: IEventBus.StopAsync now accepts optional CancellationToken for graceful shutdown (non-breaking).

- WorkFlows: FlowRunnerBase.ExecuteNodeAsync now forwards CancellationToken to sync nodes (INode) via new INode.Execute(Context, CancellationToken) overload; existing Execute(Context) remains and delegates (non-breaking API addition).

v.1.1.0 (2025-09-14)
- New: Introduce event subsystem under Magnett.Automation.Core.Events (Event, EventBus, EventStream, registries, consumers, options).
- New: Add node lifecycle events under WorkFlows.Runtimes.Events (OnNodeInit/Execute/Completed/Failed/Cancelled/Paused) and supporting types (NodeAction, NodeState, ExitState).
- Change: Workflows runtime now emits lifecycle events from NodeBase/Node/NodeAsync and FlowRunner; update IFlow/IFlowRunner/INode*; adjust NodeFactory; refine NodeExit/NodeLinkKey.
- Change: StateMachines add OnMachineInit and OnMachineChangeState events; updates to builders/collections/interfaces/exceptions; minor tweaks to Machine/State/Transition.
- Change: Contexts add OnChangeFieldValueEvent; refactor Context/ContextField/ContextVault; adjust IContextVault.
- Change: Workflow Definitions update FlowDefinition, IFlowDefinition and INodeDefinition; minor builder changes.
- Change: Commons refine CommonNamedKey, DictionaryWrapper, Enumeration.
- Tests: Update integration and unit tests; add event subsystem tests and fakes; rename OrderStateDefinition→OrderDefinition; add PaymentDefinition; remove PaymetStateDefinition; add Codes (ExitCode, PaymentStateCode); remove obsolete ContextManagerTest.
- Docs & assets: Add CANCELLATION-AUDIT.md and IMPROVEMENT-PLAN.md; update RELEASE-NOTES.MD; update assets/Logo.png and add assets/Logo2.png.
- Notes: API additions are backward compatible (no breaking changes).
