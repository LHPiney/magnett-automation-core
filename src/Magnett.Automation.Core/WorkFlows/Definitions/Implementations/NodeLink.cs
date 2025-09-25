using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Definitions.Implementations;

public class NodeLink : INodeLink
{
    public CommonNamedKey Key { get; }
    public CommonNamedKey FromNodeKey { get; }
    public CommonNamedKey ToNodeKey { get; }
    public string Code { get; }

    private NodeLink(
        CommonNamedKey fromNodeKey,
        CommonNamedKey toNodeKey,
        string code)
    {
        if (string.IsNullOrEmpty(code)) throw new ArgumentNullException(nameof(code));
            
        FromNodeKey = fromNodeKey
                      ?? throw new ArgumentNullException(nameof(fromNodeKey));
        ToNodeKey = toNodeKey
                    ?? throw new ArgumentNullException(nameof(toNodeKey));
        Code = code;
            
        //Generate Unique key
        Key = NodeLinkKey.Create(FromNodeKey, code);
    }

    /// <summary>
    /// Creates a new NodeLink instance connecting two nodes.
    /// </summary>
    /// <param name="fromNodeKey">The source node key.</param>
    /// <param name="toNodeKey">The target node key.</param>
    /// <param name="code">The link code identifier.</param>
    /// <returns>A new NodeLink instance.</returns>
    public static INodeLink Create(
        CommonNamedKey fromNodeKey,
        CommonNamedKey toNodeKey,
        string code)
    {
        return new NodeLink(fromNodeKey, toNodeKey, code);
    }
}