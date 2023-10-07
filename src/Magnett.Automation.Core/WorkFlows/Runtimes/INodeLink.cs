namespace Magnett.Automation.Core.WorkFlows.Runtimes;

public interface INodeLink
{
    CommonNamedKey Key { get; }
    CommonNamedKey FromNodeKey { get; }
    CommonNamedKey ToNodeKey { get; }
    string Code { get; }
}