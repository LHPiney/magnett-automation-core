using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions;

internal record NodeName : CommonNamedKey 
{
    public static readonly NodeName CreateOrder   = new(nameof(CreateOrder));
    public static readonly NodeName ConfirmOrder  = new(nameof(ConfirmOrder));
    public static readonly NodeName CancelOrder   = new(nameof(CancelOrder));
        
    public static readonly NodeName PreAuthorizePayment = new(nameof(PreAuthorizePayment));
    public static readonly NodeName ConfirmPayment      = new(nameof(ConfirmPayment));
    public static readonly NodeName CancelPayment       = new(nameof(CancelPayment));
        
    private NodeName(string name) : base(name)
    {
            
    }
}