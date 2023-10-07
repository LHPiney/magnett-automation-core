namespace Magnett.Automation.Core.WorkFlows.Runtimes.Builders;

public interface INodeLinkBuilder
{
    IGotoNodeLinkBuilder OnExitCode(Enumeration code);
    IGotoNodeLinkBuilder OnExitCode(string code);
    FlowDefinitionBuilder Build();
}