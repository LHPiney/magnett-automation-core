namespace Magnett.Automation.Core.WorkFlows.Builders;

public interface IGotoNodeLinkBuilder
{
    INodeLinkBuilder GoTo(CommonNamedKey toNodeKey);
}