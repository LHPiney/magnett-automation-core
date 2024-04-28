namespace Magnett.Automation.Core.WorkFlows.Definitions.Builders;

public interface IGotoNodeLinkBuilder
{
    INodeLinkBuilder GoTo(CommonNamedKey toNodeKey);
}