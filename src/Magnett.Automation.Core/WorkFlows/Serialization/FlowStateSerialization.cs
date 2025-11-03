using System.Security.Cryptography;

namespace Magnett.Automation.Core.WorkFlows.Serialization;

public static class FlowStateSerialization
{
    public static (string codecId, byte[] bytes, string sha256) ToBytes(
        IReadOnlyDictionary<string, object?> state,
        string? codecId = null)
    {
        var codec = FlowCodecFactory.Get(codecId);
        var data = codec.Serialize(state);
        return (codec.Id, data.ToArray(), Sha256(data));
    }

    public static IReadOnlyDictionary<string, object?> FromBytes(string codecId, ReadOnlySpan<byte> bytes)
    {
        var codec = FlowCodecFactory.Get(codecId);
        return codec.Deserialize<Dictionary<string, object?>>(bytes);
    }

    static string Sha256(ReadOnlyMemory<byte> content)
    {
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(content.ToArray()));
    }
}