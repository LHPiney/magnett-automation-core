using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Nodes;

internal abstract class Common : Node
{
    protected ContextDefinition ContextDefinition { get; }

    protected Common(CommonNamedKey key, ContextDefinition contextDefinition) : base(key)
    {
        ContextDefinition = contextDefinition
                            ?? throw new ArgumentNullException(nameof(contextDefinition));
    }
}