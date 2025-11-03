using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Definitions.Serialization.Models;

namespace Magnett.Automation.Core.WorkFlows.Definitions.Loaders.Implementations;

public class JsonFlowDefinitionLoader : IFlowDefinitionLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        AllowTrailingCommas = false,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    public string Format => "json";
    public string[] SupportedExtensions => new[] { ".json" };

    public async Task<Result<FlowDefinitionModel>> LoadDocumentAsync(Stream stream, CancellationToken ct = default)
    {
        try
        {
            if (stream == null)
            {
                return Result<FlowDefinitionModel>.Failure("Stream cannot be null");
            }

            if (!stream.CanRead)
            {
                return Result<FlowDefinitionModel>.Failure("Stream is not readable");
            }

            var model = await JsonSerializer.DeserializeAsync<FlowDefinitionModel>(
                stream, 
                JsonOptions, 
                ct);

            if (model == null)
            {
                return Result<FlowDefinitionModel>.Failure("Failed to deserialize JSON document");
            }

            return Result<FlowDefinitionModel>.Success(model);
        }
        catch (JsonException ex)
        {
            return Result<FlowDefinitionModel>.Failure($"Invalid JSON format: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            return Result<FlowDefinitionModel>.Failure($"Error loading document: {ex.Message}", ex);
        }
    }

    public async Task<Result<FlowDefinitionModel>> LoadDocumentAsync(string filePath, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Result<FlowDefinitionModel>.Failure("File path cannot be null or empty");
            }

            if (!File.Exists(filePath))
            {
                return Result<FlowDefinitionModel>.Failure($"File not found: {filePath}");
            }

            await using var fileStream = new FileStream(
                filePath, 
                FileMode.Open, 
                FileAccess.Read, 
                FileShare.Read,
                4096, 
                useAsync: true);

            return await LoadDocumentAsync(fileStream, ct);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Result<FlowDefinitionModel>.Failure($"Access denied to file: {filePath}", ex);
        }
        catch (Exception ex)
        {
            return Result<FlowDefinitionModel>.Failure($"Error reading file '{filePath}': {ex.Message}", ex);
        }
    }

    public async Task<Result> SaveDocumentAsync(FlowDefinitionModel model, Stream stream, CancellationToken ct = default)
    {
        try
        {
            if (model == null)
            {
                return Result.Failure("Model cannot be null");
            }

            if (stream == null)
            {
                return Result.Failure("Stream cannot be null");
            }

            if (!stream.CanWrite)
            {
                return Result.Failure("Stream is not writable");
            }

            await JsonSerializer.SerializeAsync(
                stream, 
                model, 
                JsonOptions, 
                ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error saving model: {ex.Message}", ex);
        }
    }

    public async Task<Result> SaveDocumentAsync(FlowDefinitionModel model, string filePath, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Result.Failure("File path cannot be null or empty");
            }

            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using var fileStream = new FileStream(
                filePath, 
                FileMode.Create, 
                FileAccess.Write, 
                FileShare.None,
                4096, 
                useAsync: true);

            return await SaveDocumentAsync(model, fileStream, ct);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Result.Failure($"Access denied to file: {filePath}", ex);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error writing file '{filePath}': {ex.Message}", ex);
        }
    }
}

