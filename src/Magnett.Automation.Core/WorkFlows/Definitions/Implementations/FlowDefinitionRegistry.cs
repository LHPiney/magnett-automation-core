using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Definitions.Cache;
using Magnett.Automation.Core.WorkFlows.Definitions.Cache.Implementations;
using Magnett.Automation.Core.WorkFlows.Definitions.Loaders;
using Magnett.Automation.Core.WorkFlows.Definitions.Loaders.Implementations;
using Magnett.Automation.Core.WorkFlows.Definitions.Serialization;
using Magnett.Automation.Core.WorkFlows.Definitions.Validation;
using Magnett.Automation.Core.WorkFlows.Definitions.Validation.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Definitions.Implementations;

public sealed class FlowDefinitionRegistry : IFlowDefinitionRegistry
{
    private readonly Dictionary<string, IFlowDefinitionLoader> _loaders = new(StringComparer.OrdinalIgnoreCase);
    private IFlowDefinitionCache? _cache;
    private IFlowDefinitionValidator? _validator;

    public FlowDefinitionRegistry()
    {
        RegisterLoader(new JsonFlowDefinitionLoader());
        _cache = new InMemoryFlowDefinitionCache();
        _validator = new FlowDefinitionValidator();
    }

    public void RegisterLoader(IFlowDefinitionLoader loader)
    {
        if (loader == null)
            throw new ArgumentNullException(nameof(loader));

        foreach (var extension in loader.SupportedExtensions)
        {
            var normalizedExtension = extension.StartsWith(".") ? extension : $".{extension}";
            _loaders[normalizedExtension] = loader;
        }
    }

    public void SetCache(IFlowDefinitionCache? cache)
    {
        _cache = cache;
    }

    public void SetValidator(IFlowDefinitionValidator? validator)
    {
        _validator = validator;
    }

    public async Task<Result<IFlowDefinition>> LoadAsync(string filePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return Result<IFlowDefinition>.Failure("File path cannot be null or empty");
        }

        var extension = Path.GetExtension(filePath);
        if (string.IsNullOrEmpty(extension))
        {
            return Result<IFlowDefinition>.Failure($"File path '{filePath}' does not have an extension");
        }

        if (!_loaders.TryGetValue(extension, out var loader))
        {
            return Result<IFlowDefinition>.Failure($"No loader registered for extension '{extension}'");
        }

        var modelResult = await loader.LoadDocumentAsync(filePath, ct);
        if (modelResult.IsFailure)
        {
            return Result<IFlowDefinition>.Failure(modelResult.ErrorMessage ?? "Failed to load model", modelResult.Exception);
        }

        var model = modelResult.Value!;

        if (_validator != null)
        {
            var validationResult = _validator.Validate(model);
            if (validationResult.IsFailure)
            {
                return Result<IFlowDefinition>.Failure(validationResult.ErrorMessage ?? "Validation failed", validationResult.Exception);
            }
        }

        try
        {
            var definition = FlowDefinitionModelMapper.ToFlowDefinition(model);

            if (_cache != null)
            {
                await _cache.SetAsync(definition, ct);
            }

            return Result<IFlowDefinition>.Success(definition);
        }
        catch (Exception ex)
        {
            return Result<IFlowDefinition>.Failure($"Error converting model to FlowDefinition: {ex.Message}", ex);
        }
    }

    public async Task<Result<IFlowDefinition>> GetAsync(string key, int? version = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result<IFlowDefinition>.Failure("Key cannot be null or empty");
        }

        if (_cache == null)
        {
            return Result<IFlowDefinition>.Failure("Cache is not configured");
        }

        var definition = await _cache.GetAsync(key, version, ct);
        if (definition == null)
        {
            return Result<IFlowDefinition>.Failure($"Flow definition with key '{key}' and version {version?.ToString() ?? "latest"} not found in cache");
        }

        return Result<IFlowDefinition>.Success(definition);
    }

    public async Task<Result<IFlowDefinition>> GetAsync(CommonNamedKey key, int? version = null, CancellationToken ct = default)
    {
        if (key == null)
        {
            return Result<IFlowDefinition>.Failure("Key cannot be null");
        }

        return await GetAsync(key.Name, version, ct);
    }

    public async Task<Result> RegisterAsync(IFlowDefinition definition, CancellationToken ct = default)
    {
        if (definition == null)
        {
            return Result.Failure("Definition cannot be null");
        }

        if (_cache == null)
        {
            return Result.Failure("Cache is not configured");
        }

        await _cache.SetAsync(definition, ct);
        return Result.Success();
    }

    public static FlowDefinitionRegistry Create()
    {
        return new FlowDefinitionRegistry();
    }
}

