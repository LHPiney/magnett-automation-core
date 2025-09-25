using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Codes;

public record PaymentStateCode : Enumeration
{
    public static readonly PaymentStateCode New = new(1, nameof(New));
    public static readonly PaymentStateCode PreAuthorized = new(2, nameof(PreAuthorized));
    public static readonly PaymentStateCode Denied = new(3, nameof(Denied));
    public static readonly PaymentStateCode Confirmed = new(4, nameof(Confirmed));
    public static readonly PaymentStateCode Cancelled = new(5, nameof(Cancelled));

    private PaymentStateCode(int id, string name) : base(id, name)
    {
    }
}