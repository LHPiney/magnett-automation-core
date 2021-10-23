using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions
{
    internal class Node : CommonNamedKey 
    {
        public static readonly Node Reset     = new Node("Reset");
        public static readonly Node SetValue  = new Node("SetValue");
        public static readonly Node SumValue  = new Node("SumValue");
        
        private Node(string name) : base(name)
        {
            
        }
    }
}