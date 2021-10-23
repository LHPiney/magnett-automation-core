using Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Nodes;
using Magnett.Automation.Core.WorkFlows;
using Magnett.Automation.Core.WorkFlows.Builders;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions
{
    public static class SimpleFlowDefinition
    {
        private static IFlowDefinition   _definition;
        private static ContextDefinition _contextDefinition;
        
        static SimpleFlowDefinition()
        {
            CreateDefinition();
        }
        
        private static void CreateDefinition()
        {
            _contextDefinition = ContextDefinition.Create();
            
            _definition = FlowDefinitionBuilder.Create()
                .WithInitialNode(ResetValue.Create(Node.Reset, _contextDefinition))
                    .OnExitCode(ResetValue.ExitCode.Ok).GoTo(Node.SetValue)
                .Build()
                
                .WithNode(SetValue.Create(Node.SetValue, _contextDefinition))
                    .OnExitCode(SetValue.ExitCode.Assigned).GoTo(Node.SumValue)
                .Build()
                
                .WithNode(SumValue.Create(Node.SumValue, _contextDefinition)).Build()
                
                .BuildDefinition();
        }

        public static IFlowDefinition GetDefinition()
        {
            return _definition;
        }
    }
}