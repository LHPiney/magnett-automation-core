using System;
using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Nodes
{
    internal abstract class Common : Core.WorkFlows.Implementations.Node
    {
        protected ContextDefinition ContextDefinition { get; }

        protected Common(CommonNamedKey key, ContextDefinition contextDefinition) : base(key)
        {
            ContextDefinition = contextDefinition
                                ?? throw new ArgumentNullException(nameof(contextDefinition));
        }
    }
}