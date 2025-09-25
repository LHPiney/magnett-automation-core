using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Codes;

public record ExitCode : Enumeration
{
    public static readonly ExitCode Done          = new(1, nameof(Done)); 
    public static readonly ExitCode Created       = new(2, nameof(Created));
    public static readonly ExitCode PreAuthorized = new(3, nameof(PreAuthorized));
    public static readonly ExitCode Denied        = new(4, nameof(Denied));
    public static readonly ExitCode Cancelled     = new(5, nameof(Cancelled));
    public static readonly ExitCode Failed        = new(6, nameof(Failed)); 

    private ExitCode(int id, string name) : base(id, name)
    {
    }
}
