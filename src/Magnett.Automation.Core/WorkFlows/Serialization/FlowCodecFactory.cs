using System.Collections.Concurrent;
using Magnett.Automation.Core.WorkFlows.Serialization.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Serialization;

public static class FlowCodecFactory
{
    static readonly ConcurrentDictionary<string, IFlowCodec> _codecs =
        new(StringComparer.OrdinalIgnoreCase);

    static FlowCodecFactory()
    {
        Register(new JsonFlowCodec());
    }

    public static void Register(IFlowCodec codec)
    {
        _codecs[codec.Id] = codec;
    }

    public static IFlowCodec Get(string? id)
    {
        if (string.IsNullOrWhiteSpace(id)) return _codecs["json"];
        return _codecs.TryGetValue(id, out var c) 
            ? c 
            : throw new InvalidOperationException($"Codec '{id}' is not registered.");
    }
}