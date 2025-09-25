namespace Magnett.Automation.Core.WorkFlows.Runtimes;

public record ExitState(int Id, string Name) : Enumeration(Id, Name)
{
    private const int CompletedId = 1;
    private const int FailedId = 2;
    private const int PausedId = 3;
    private const int CancelledId = 4;
    
    public static readonly ExitState Completed = new(CompletedId,nameof(Completed));
    public static readonly ExitState Failed    = new(FailedId, nameof(Failed));
    public static readonly ExitState Paused    = new(PausedId, nameof(Paused));
    public static readonly ExitState Cancelled = new(CancelledId, nameof(Cancelled));
}