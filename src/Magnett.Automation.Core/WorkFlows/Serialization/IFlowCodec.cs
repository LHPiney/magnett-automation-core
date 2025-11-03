namespace Magnett.Automation.Core.WorkFlows.Serialization;

public interface IFlowCodec
{
    string Id { get; }
    ReadOnlyMemory<byte> Serialize<T>(T value);
    T Deserialize<T>(ReadOnlySpan<byte> bytes);
}