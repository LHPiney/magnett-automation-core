using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Runtimes;

namespace Magnett.Automation.Server.Repositories.Implementations;

public class InMemoryFlowRepository : IFlowRepository
{
    private readonly ConcurrentDictionary<Guid, IFlow> _flows = new();
    private readonly ConcurrentDictionary<Guid, FlowState> _states = new();

    public Task<Result> SaveAsync(IFlow flow, FlowState state, CancellationToken ct)
    {
        if (flow == null)
        {
            return Task.FromResult(Result.Failure("Flow cannot be null"));
        }

        _flows.AddOrUpdate(flow.Id, flow, (_, _) => flow);
        _states.AddOrUpdate(flow.Id, state, (_, _) => state);

        return Task.FromResult(Result.Success());
    }

    public Task<Result<IFlow>> GetAsync(Guid id, CancellationToken ct)
    {
        if (_flows.TryGetValue(id, out var flow))
        {
            return Task.FromResult(Result<IFlow>.Success(flow));
        }

        return Task.FromResult(Result<IFlow>.Failure($"Flow with id '{id}' not found"));
    }

    public Task<Result<FlowState>> GetStateAsync(Guid id, CancellationToken ct)
    {
        if (_states.TryGetValue(id, out var state))
        {
            return Task.FromResult(Result<FlowState>.Success(state));
        }

        return Task.FromResult(Result<FlowState>.Failure($"Flow state with id '{id}' not found"));
    }

    public Task<Result<IEnumerable<IFlow>>> GetByStateAsync(FlowState state, CancellationToken ct)
    {
        var flows = _flows
            .Where(kvp => _states.TryGetValue(kvp.Key, out var flowState) && flowState == state)
            .Select(kvp => kvp.Value)
            .ToList();

        return Task.FromResult(Result<IEnumerable<IFlow>>.Success(flows));
    }

    public Task<Result> UpdateStateAsync(Guid id, FlowState state, CancellationToken ct)
    {
        if (!_states.ContainsKey(id))
        {
            return Task.FromResult(Result.Failure($"Flow with id '{id}' not found"));
        }

        _states.AddOrUpdate(id, state, (_, _) => state);

        return Task.FromResult(Result.Success());
    }

    public Task<Result> DeleteAsync(Guid id, CancellationToken ct)
    {
        var flowRemoved = _flows.TryRemove(id, out _);
        var stateRemoved = _states.TryRemove(id, out _);

        if (!flowRemoved && !stateRemoved)
        {
            return Task.FromResult(Result.Failure($"Flow with id '{id}' not found"));
        }

        return Task.FromResult(Result.Success());
    }

    public Task<Result<bool>> ExistsAsync(Guid id, CancellationToken ct)
    {
        var exists = _flows.ContainsKey(id);
        return Task.FromResult(Result<bool>.Success(exists));
    }

    public static InMemoryFlowRepository Create()
    {
        return new InMemoryFlowRepository();
    }
}

