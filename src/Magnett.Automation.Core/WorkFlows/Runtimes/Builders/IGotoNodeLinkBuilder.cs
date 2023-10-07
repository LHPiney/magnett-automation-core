namespace Magnett.Automation.Core.WorkFlows.Runtimes.Builders;

public interface IGotoNodeLinkBuilder
{
    INodeLinkBuilder GoTo(CommonNamedKey toNodeKey);
}