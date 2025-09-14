# Magnett.Automation.Core — Improvement Plan (2025-09-14)

This document reviews the current solution and proposes a concrete, actionable improvement plan grouped by namespaces. It prioritizes developer ergonomics, correctness, observability, and extensibility while minimizing breaking changes.

Scope: Source under `Magnett.Automation.Core` and related tests. Non‑goals: rewrite of public API; radical architecture changes.

## High‑level findings
- Clear domain split: Commons, Contexts, Events, StateMachines, WorkFlows. Tests exist for units and integrations.
- Good use of composition (EventBus, factories) and fluent builders.
- Observed items to improve:
  - In WorkFlows.Runtimes.NodeBase, failure path emits `OnNodeCompletedEvent` for failed state (likely should be `OnNodeFailedEvent`).
  - Cancellation support is inconsistent (StateMachine Dispatch has no `CancellationToken`; WorkFlows have partial support).
  - Validation and error messages can be more explicit in builders and definitions.
  - Cross‑cutting: nullability annotations, XML docs, and logging consistency.

## Priorities
1. Correctness & safety (validation, error handling, thread-safety).
2. Observability (logging, events taxonomy, error codes).
3. Developer Experience (APIs consistency, docs, samples).
4. Performance (only where measured; avoid premature optimization).

---

## Namespace plans and concrete tasks

### Magnett.Automation.Core.Commons
Goals: strengthen primitives and helpers.

Tasks
- CommonNamedKey
  - Ensure complete equality semantics and case sensitivity policy documented (acceptance: unit tests for `Equals`, `GetHashCode`, case behavior).
  - Add `TryParse(string, out CommonNamedKey)` helper (acceptance: parses valid names, rejects null/empty; no exceptions on invalid input).
- Enumeration
  - Verify comparison and serialization behavior (acceptance: JSON round‑trip tests and equality tests).
- DictionaryWrapper / IDictionaryWrapper
  - Review thread-safety assumptions; consider marking as not thread-safe or provide a `ConcurrentDictionary`-backed variant (acceptance: docs + dedicated tests).
- Nullability
  - Enable annotations consistently and remove redundant null checks where the type system guarantees non-null (acceptance: solution builds with nullable enabled and no warnings in Commons).

### Magnett.Automation.Core.Contexts
Goals: predictable state management, eventing, and concurrency.

Tasks
- Context API surface
  - Add `Remove`, `Clear`, `Contains` operations to `Context` via vault (acceptance: unit tests verifying behavior and events emitted/not emitted).
  - Add batch `StoreAsync(IEnumerable<(Field,Value)>)` to minimize event bursts (acceptance: emits per-field events or a single batch event — decision documented and tested).
  - Provide immutable snapshot view (e.g., `ToSnapshot()` returning read-only dictionary) (acceptance: snapshot is detached from subsequent changes).
- Eventing
  - Debounce/throttle option for high-churn fields; document default (acceptance: tests show event coalescing when enabled).
  - Ensure `OnChangeFieldValueEvent` includes old/new values for reference types safely (null handling) (acceptance: null edge cases covered).
- Concurrency
  - Review and document locking strategy (`_syncRoot`), ensure no deadlocks across `EmitEventAsync` (emit outside lock; currently ok) (acceptance: concurrency tests with parallel stores).
- Cancellation
  - Flow `CancellationToken` through `StoreAsync` to `EmitEventAsync` (already present) and through any new APIs (acceptance: cancellation respected and tested).

### Magnett.Automation.Core.Events
Goals: consistent event taxonomy and robust bus interaction.

Tasks
- Event taxonomy
  - Define categories (Context, StateMachine, Workflow.Node, Workflow.Runner) and standard properties (Id, CorrelationId, CausationId, Severity) (acceptance: base `Event` extended; adapters updated; docs added).
- Event identifiers
  - Introduce `EventId` (from Microsoft.Extensions.Logging) mapping or internal code enum for known events (acceptance: mapping table + tests).
- EventBus surface
  - Clarify publish semantics (fire-and-forget vs. guaranteed delivery) and add retry policy hooks (acceptance: interface docs + in-memory implementation with optional retry policy function).
- EventReader
  - Ensure buffering/backpressure policy documented; add capacity option for ring buffer if applicable (acceptance: tests for overflow policy).

### Magnett.Automation.Core.StateMachines.Builders
Goals: safer, clearer builder flow and diagnostics.

Tasks
- Validation
  - Builder should detect: duplicate state names, missing transitions, unreachable states, missing initial state (acceptance: exceptions with actionable messages; unit tests cover each case).
- Fluent API ergonomics
  - Overloads accept `Enumeration` or `string` consistently; add `ToState(Enumeration)` overloads (acceptance: compile-time coverage + samples).
- Diagnostics
  - Enrich exception types/messages (include action/state names) (acceptance: snapshot tests on exception messages).

### Magnett.Automation.Core.StateMachines.Implementations
Goals: predictable transitions, cancel support, clearer errors, events.

Tasks
- Machine
  - Add `CancellationToken` to `DispatchAsync` overloads; preserve existing API as convenience wrapper (acceptance: token short-circuits execution; tests).
  - Improve error messages for `InvalidOperationException` (include machine id/current state) (acceptance: assertion on message).
  - Guard `OnTransitionAsync` exceptions and surface as domain exception with context (acceptance: unit test asserts wrapping and original cause preserved).
  - Telemetry hook: optional logger or event for transition attempted/failed (acceptance: emitted around transitions and covered by tests).
- Exceptions
  - Review `StateNotFoundException`, `InvalidMachineDefinitionException` messages and add `ToString` details (acceptance: message includes state/action names).

### Magnett.Automation.Core.WorkFlows.Definitions
Goals: correctness checks for workflow definitions.

Tasks
- Validation
  - Detect missing initial node; unknown node references; cycles as per business rules; dangling transitions (acceptance: definition builder throws with precise diagnostics; tests for each case).
- Ergonomics
  - Add helpers to reference nodes by typed keys; provide compile-time safety via `CommonNamedKey` where possible (acceptance: samples compile and run).

### Magnett.Automation.Core.WorkFlows.Runtimes
Goals: robust runner lifecycle, node semantics, and events.

Tasks
- FlowRunnerBase / IFlowRunner
  - Define lifecycle: `Start`, `Stop`, `Dispose` (if needed); surface `CancellationToken` uniformly (acceptance: runner can be stopped; integration tests prove graceful cancellation).
  - Logging consistency (structured logs with node names, timings) (acceptance: logs verified via test sink).
  - Event collection (`Events`) ordering already by `CreatedAt`; document guarantees (acceptance: doc + small test).
- NodeBase
  - BUG: Failure path currently emits `OnNodeCompletedEvent` on failure; replace with `OnNodeFailedEvent` (acceptance: unit test verifying correct event on failure; migration note in release docs).
  - Strengthen `ManageResponse` to handle unknown codes with explicit error (acceptance: test).
  - Initialization: `Init` should be idempotent and safe to call concurrently (acceptance: parallel test shows single state machine created and transitions applied once).
  - Expose a protected `TryEmitNodeEvent` with safe exception handling (acceptance: event failures do not crash node unless configured).

### Cross‑cutting concerns
- Nullability and analyzers
  - Enable nullable context across projects; fix warnings (acceptance: 0 nullable warnings in CI).
  - Add Roslyn analyzers (e.g., IDISP, CA, StyleCop optional) (acceptance: CI enforces rules; baseline set documented).
- Logging
  - Ensure all public operations log at Debug/Information with structured templates (acceptance: smoke tests validate key paths log expected entries).
- Docs & samples
  - XML docs for public APIs; generate docfx or similar (acceptance: build step generates docs without warnings).
  - Add minimal runnable samples for StateMachine and Workflow under `samples/` (acceptance: CI builds samples).
- Packaging
  - Confirm target frameworks (prefer net8.0 LTS + others if needed); update NuGet metadata (acceptance: `dotnet pack` passes; README badges updated).
- CI/CD
  - Update `azure-pipelines.yml` to run unit + integration tests, collect coverage, and publish artifacts (acceptance: pipeline green with coverage summary).

### Testing strategy (by namespace)
- Commons: value semantics and edge cases.
- Contexts: concurrency, event emission logic.
- Events: publish/subscribe behavior, retry hooks.
- StateMachines: transition tables, invalid inputs, async transition handlers.
- WorkFlows: runner lifecycle, node event sequences, integration happy-path and failure-path flows.

### Migration & versioning
- Changes that adjust events (e.g., failure event type) are minor-breaking in behavior; bump minor version (e.g., 0.7.0) and document in RELEASE-NOTES.
- Provide upgrade notes for any API additions (overloads) and new validations that may throw earlier.

---

## Execution roadmap (phased)
1. Phase 1 (Correctness)
   - NodeBase failure event fix; builder validations; exception message improvements; basic tests.
2. Phase 2 (Observability)
   - Logging standardization; event taxonomy; test sink verification.
3. Phase 3 (DX & Docs)
   - API overloads, snapshots, samples, XML docs.
4. Phase 4 (Polish)
   - Nullable cleanup, analyzers, CI enhancements, packaging.

Owners and timelines can be assigned in issues mapped 1:1 to the tasks above.
