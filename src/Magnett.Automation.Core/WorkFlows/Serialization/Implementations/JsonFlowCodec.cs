using System.Text.Json;
using System.Text.Json.Serialization;

namespace Magnett.Automation.Core.WorkFlows.Serialization.Implementations;

public sealed class JsonFlowCodec : IFlowCodec
{
    private static readonly JsonSerializerOptions Opt = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public string Id => "json";

    public ReadOnlyMemory<byte> Serialize<T>(T value) => JsonSerializer.SerializeToUtf8Bytes(value, Opt);

    public T Deserialize<T>(ReadOnlySpan<byte> bytes)
    {
        var result = JsonSerializer.Deserialize<T>(bytes, Opt) 
            ?? throw new InvalidOperationException($"Failed to deserialize JSON to type {typeof(T).Name}");
        
        return result;
    }
}