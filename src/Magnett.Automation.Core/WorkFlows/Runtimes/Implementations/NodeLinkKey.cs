[assembly: InternalsVisibleTo("Magnett.Automation.Core.UnitTest")]
namespace Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

internal record NodeLinkKey : CommonNamedKey 
{
    private NodeLinkKey(string name) : base(name)
    {
            
    }

    private NodeLinkKey(CommonNamedKey nodeKey, string code) : this($"{nodeKey?.Name}.{code}")
    {
        if (nodeKey == null) throw new ArgumentNullException(nameof(nodeKey));
        if (string.IsNullOrEmpty(code)) throw new ArgumentNullException(nameof(code));
    }

    public static NodeLinkKey Create(CommonNamedKey nodeKey, string code)
    {
        return new NodeLinkKey(nodeKey, code);
    }
}