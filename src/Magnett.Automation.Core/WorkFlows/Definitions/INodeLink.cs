namespace Magnett.Automation.Core.WorkFlows.Definitions;

public interface INodeLink
{
    CommonNamedKey Key { get; }
    CommonNamedKey FromNodeKey { get; }
    CommonNamedKey ToNodeKey { get; }
    string Code { get; }
}