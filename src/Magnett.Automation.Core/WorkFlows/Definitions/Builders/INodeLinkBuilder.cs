namespace Magnett.Automation.Core.WorkFlows.Definitions.Builders;

public interface INodeLinkBuilder
{
    IGotoNodeLinkBuilder OnExitCode(Enumeration code);
    IGotoNodeLinkBuilder OnExitCode(string code);
    FlowDefinitionBuilder Build();
}