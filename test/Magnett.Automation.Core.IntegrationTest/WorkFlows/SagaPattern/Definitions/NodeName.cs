using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions
{
    internal class NodeName : CommonNamedKey 
    {
        public static readonly NodeName CreateOrder   = new NodeName(nameof(CreateOrder));
        public static readonly NodeName ConfirmOrder  = new NodeName(nameof(ConfirmOrder));
        public static readonly NodeName CancelOrder   = new NodeName(nameof(CancelOrder));
        
        public static readonly NodeName PreAuthorizePayment = new NodeName(nameof(PreAuthorizePayment));
        public static readonly NodeName ConfirmPayment      = new NodeName(nameof(ConfirmPayment));
        public static readonly NodeName CancelPayment       = new NodeName(nameof(CancelPayment));
        
        private NodeName(string name) : base(name)
        {
            
        }
    }
}