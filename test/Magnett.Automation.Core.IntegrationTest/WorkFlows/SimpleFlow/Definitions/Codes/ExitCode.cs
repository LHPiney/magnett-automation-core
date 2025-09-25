using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Codes;

public record ExitCode : Enumeration
{
    public static readonly ExitCode Done      = new(1, "Done");
    public static readonly ExitCode Cancelled = new(2, "Cancelled"); 
    public static readonly ExitCode Assigned  = new(3, "Assigned");
    public static readonly ExitCode Ok        = new(4, "Ok"); 

    private ExitCode(int id, string name) : base(id, name)
    {
    }
}