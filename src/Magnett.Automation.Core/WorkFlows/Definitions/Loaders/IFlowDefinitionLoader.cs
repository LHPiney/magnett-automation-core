using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Definitions.Serialization.Models;

namespace Magnett.Automation.Core.WorkFlows.Definitions.Loaders;

public interface IFlowDefinitionLoader
{
    string Format { get; }
    string[] SupportedExtensions { get; }

    Task<Result<FlowDefinitionModel>> LoadDocumentAsync(Stream stream, CancellationToken ct = default);
    Task<Result<FlowDefinitionModel>> LoadDocumentAsync(string filePath, CancellationToken ct = default);
    Task<Result> SaveDocumentAsync(FlowDefinitionModel model, Stream stream, CancellationToken ct = default);
    Task<Result> SaveDocumentAsync(FlowDefinitionModel model, string filePath, CancellationToken ct = default);
}

