using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.StateMachines.Builders;

namespace Magnett.Automation.Core.WorkFlows.Builders
{
    public interface INodeLinkBuilder
    {
        IGotoNodeLinkBuilder OnExitCode(Enumeration code);
        IGotoNodeLinkBuilder OnExitCode(string code);
        FlowDefinitionBuilder Build();
    }
}