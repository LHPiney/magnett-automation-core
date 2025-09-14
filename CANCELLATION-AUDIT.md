# Cancellation Support Audit (2025-09-14)

This document pinpoints where cancellation support is missing or inconsistently propagated across the solution and proposes minimal, incremental recommendations.

Scope reviewed: Contexts, Events, StateMachines, WorkFlows (Runtimes + Definitions), and tests.

---

## Summary at a glance
- Contexts: Consistent. Store APIs accept and propagate CancellationToken.
- Events: Publish/Start/Stop accept tokens; event processing honors tokens.
- WorkFlows: Improved. Runners accept tokens; NodeBase/Node/NodeAsync now propagate tokens to state transitions' event emission. Underlying state machine Dispatch still lacks tokens (Stage B).
- StateMachines: No tokens on critical operations (Dispatch/Initialize/Restart), so cancellation can’t short-circuit transitions or emitted events.

---

## Detailed findings

### 1) Events layer
- File: src\Magnett.Automation.Core\Events\IEventBus.cs
  - PublishAsync(IEvent, CancellationToken = default) — OK
  - Start(CancellationToken = default) — OK
  - StopAsync(CancellationToken = default) — OK
- File: src\Magnett.Automation.Core\Events\Implementations\EventEmitterEntity.cs
  - EmitEventAsync(IEvent, CancellationToken = default) — OK (honors token)
- File: src\Magnett.Automation.Core\Events\Implementations\EventBus.cs
  - Start uses a linked CTS; PublishAsync and processing loop honor tokens; StopAsync awaits processing task with token — OK

Recommendation:
- None. EventBus surface is now symmetric for start/stop with CancellationToken support.

### 2) Contexts
- File: src\Magnett.Automation.Core\Contexts\Context.cs
  - StoreAsync<T>(..., CancellationToken = default) forwards token to EmitEventAsync — OK
  - TryGetValue/Value — not async (no token needed) — OK

Recommendation:
- Keep as-is. Ensure any future batch operations also accept a token and forward it to event emission.

### 3) WorkFlows — Flow runner and nodes
- File: src\Magnett.Automation.Core\WorkFlows\Runtimes\Implementations\FlowRunnerBase.cs
  - Start(CancellationToken = default) — abstract — OK
  - ExecuteNodeAsync(INodeBase, CancellationToken = default) — forwards token to both INodeAsync and INode (sync) — OK (deeper transitions still not token-aware)
- File: src\Magnett.Automation.Core\WorkFlows\Runtimes\Implementations\NodeAsync.cs
  - ExecuteAsync(Context, CancellationToken) — receives token; forwards to HandleAsync and calls TransitStateAsync(..., token) — OK
- File: src\Magnett.Automation.Core\WorkFlows\Runtimes\Implementations\Node.cs
  - Execute(Context, CancellationToken) now passes token to TransitStateAsync and ManageResponse; Execute(Context) delegates to token overload — OK
- File: src\Magnett.Automation.Core\WorkFlows\Runtimes\Implementations\NodeBase.cs
  - Init(), Initialize(), TransitStateAsync<TEvent>(...) now accept CancellationToken and forward it to EmitEventAsync; States.DispatchAsync(...) still has no token (to be addressed in Stage B) — PARTIAL OK

Effects:
- Even when a FlowRunner is canceled, node state transitions and event emissions might still occur because transitions/events are not token-aware.
- Sync nodes cannot be canceled at API level; async nodes can cancel their work (HandleAsync) but still perform transitions and event emission.

Recommendations (minimal, staged):
1) Add CancellationToken to NodeBase methods:
   - Init(CancellationToken = default)
   - Initialize(CancellationToken = default)
   - TransitStateAsync<TEvent>(..., CancellationToken = default) and use it in States.DispatchAsync(...) and EmitEventAsync(...)
2) Update NodeAsync.ExecuteAsync and Node.Execute to pass through the token to TransitStateAsync calls.
3) Consider adding a token-enabled overload for Node.Execute(Context, CancellationToken = default) and default the old API to call the new one with default.
4) In FlowRunnerBase.ExecuteNodeAsync, consider adding a path for INode that accepts token (once overload exists), otherwise document that sync nodes are not cancellable.

### 4) StateMachines — Machine
- File: src\Magnett.Automation.Core\StateMachines\Implementations\Machine.cs
  - CreateAsync(...), InitializeAsync(), ReStartAsync() — no CancellationToken — INCONSISTENT
  - DispatchAsync(Enumeration) and DispatchAsync(string) — no CancellationToken — INCONSISTENT
  - Transit(...) — no token; may await transition.OnTransitionAsync without a token — INCONSISTENT
  - EmitEventAsync calls (OnMachineInit, OnMachineChangeState) are invoked without an external token — INCONSISTENT

Effects:
- Workflows cannot cancel underlying state transitions (node states), even if higher layers are canceled.
- Event emissions from state machine operations cannot be canceled.

Recommendations (minimal, staged):
1) Introduce token-enabled overloads while preserving existing signatures:
   - DispatchAsync(Enumeration, CancellationToken = default)
   - DispatchAsync(string, CancellationToken = default)
   - CreateAsync(IMachineDefinition, IEventBus? = null, CancellationToken = default)
   - ReStartAsync(CancellationToken = default)
   - InitializeAsync(CancellationToken = default) [protected]
   - Internal Transit(ITransition, CancellationToken) to pass down the token.
2) Flow tokens to EmitEventAsync and to any OnTransitionAsync delegate (consider a token parameter or a pattern to use).

---

## Proposed incremental plan (non-breaking staged)
1) Stage A — Propagate tokens in Workflow layer:
   - Add CancellationToken parameters to NodeBase.TransitStateAsync/Init/Initialize; thread through Node and NodeAsync execution paths; FlowRunnerBase remains stable.
2) Stage B — Add token-enabled overloads to Machine while keeping old methods as wrappers calling new overloads with default tokens.
3) Stage C — Consider adding CancellationToken to IEventBus.StopAsync for symmetry; keep old method for backward compatibility, or add optional parameter.

## Acceptance ideas
- Unit tests ensuring that, when CancellationToken is canceled before calling, transitions and event emissions are skipped.
- Integration tests where FlowRunner cancellation stops long-running async nodes and prevents post-cancel transitions/events.

---

This audit should guide focused PRs to make cancellation support consistent without immediate breaking changes.