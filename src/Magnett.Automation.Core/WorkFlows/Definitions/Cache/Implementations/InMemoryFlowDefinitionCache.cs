using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.WorkFlows.Definitions;

namespace Magnett.Automation.Core.WorkFlows.Definitions.Cache.Implementations;

public class InMemoryFlowDefinitionCache : IFlowDefinitionCache
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, IFlowDefinition>> _cache = new();

    public Task<IFlowDefinition?> GetAsync(string key, int? version = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Task.FromResult<IFlowDefinition?>(null);
        }

        if (!_cache.TryGetValue(key, out var versions))
        {
            return Task.FromResult<IFlowDefinition?>(null);
        }

        if (version.HasValue)
        {
            return Task.FromResult(versions.TryGetValue(version.Value, out var definition) ? definition : null);
        }

        if (versions.IsEmpty)
        {
            return Task.FromResult<IFlowDefinition?>(null);
        }

        var latestVersion = versions.Keys.Max();
        return Task.FromResult<IFlowDefinition?>(versions[latestVersion]);
    }

    public Task SetAsync(IFlowDefinition definition, CancellationToken ct = default)
    {
        if (definition == null)
        {
            return Task.CompletedTask;
        }

        var key = definition.Key.Name;
        var versions = _cache.GetOrAdd(key, _ => new ConcurrentDictionary<int, IFlowDefinition>());
        versions.AddOrUpdate(definition.Version, definition, (_, _) => definition);

        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(string key, int? version = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Task.FromResult(false);
        }

        if (!_cache.TryGetValue(key, out var versions))
        {
            return Task.FromResult(false);
        }

        if (version.HasValue)
        {
            var removed = versions.TryRemove(version.Value, out _);
            if (versions.IsEmpty)
            {
                _cache.TryRemove(key, out _);
            }
            return Task.FromResult(removed);
        }

        return Task.FromResult(_cache.TryRemove(key, out _));
    }

    public Task ClearAsync(CancellationToken ct = default)
    {
        _cache.Clear();
        return Task.CompletedTask;
    }
}

