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
