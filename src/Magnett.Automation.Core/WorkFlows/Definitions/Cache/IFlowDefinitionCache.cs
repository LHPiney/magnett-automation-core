using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.WorkFlows.Definitions;

namespace Magnett.Automation.Core.WorkFlows.Definitions.Cache;

public interface IFlowDefinitionCache
{
    Task<IFlowDefinition?> GetAsync(string key, int? version = null, CancellationToken ct = default);
    Task SetAsync(IFlowDefinition definition, CancellationToken ct = default);
    Task<bool> RemoveAsync(string key, int? version = null, CancellationToken ct = default);
    Task ClearAsync(CancellationToken ct = default);
}

