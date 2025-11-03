using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Definitions.Cache;
using Magnett.Automation.Core.WorkFlows.Definitions.Loaders;
using Magnett.Automation.Core.WorkFlows.Definitions.Validation;

namespace Magnett.Automation.Core.WorkFlows.Definitions;

public interface IFlowDefinitionRegistry
{
    void RegisterLoader(IFlowDefinitionLoader loader);
    void SetCache(IFlowDefinitionCache? cache);
    void SetValidator(IFlowDefinitionValidator? validator);

    Task<Result<IFlowDefinition>> LoadAsync(string filePath, CancellationToken ct = default);
    Task<Result<IFlowDefinition>> GetAsync(string key, int? version = null, CancellationToken ct = default);
    Task<Result> RegisterAsync(IFlowDefinition definition, CancellationToken ct = default);
    Task<Result<IFlowDefinition>> GetAsync(CommonNamedKey key, int? version = null, CancellationToken ct = default);
}

