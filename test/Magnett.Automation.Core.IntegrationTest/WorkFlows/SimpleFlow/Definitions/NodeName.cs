using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions
{
    internal class NodeName : CommonNamedKey 
    {
        public static readonly NodeName Reset     = new NodeName("Reset");
        public static readonly NodeName SetValue  = new NodeName("SetValue");
        public static readonly NodeName SumValue  = new NodeName("SumValue");
        
        private NodeName(string name) : base(name)
        {
            
        }
    }
}