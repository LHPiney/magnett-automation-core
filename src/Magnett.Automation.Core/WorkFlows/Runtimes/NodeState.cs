namespace Magnett.Automation.Core.WorkFlows.Runtimes;

public record NodeState : Enumeration
{
    private const int IdleId = 0;
    private const int ReadyId = 1;
    private const int RunningId = 2;
    private const int CompletedId = 3;
    private const int FailedId = 4;
    private const int PausedId = 5;
    private const int CancelledId = 6;
    
    public static readonly NodeState Idle = new(IdleId, nameof(Idle));
    public static readonly NodeState Ready = new(ReadyId, nameof(Ready));
    public static readonly NodeState Running = new(RunningId, nameof(Running));
    public static readonly NodeState Completed = new(CompletedId, nameof(Completed));
    public static readonly NodeState Failed = new(FailedId, nameof(Failed));
    public static readonly NodeState Paused = new(PausedId, nameof(Paused));
    public static readonly NodeState Cancelled = new(CancelledId, nameof(Cancelled));

    private NodeState(int id, string name) : base(id, name)
    {
    }

    public static NodeState WithName(CommonNamedKey? name)
    {
        if (name == null) return Idle;

        return name.Name switch
        {
            nameof(Idle)      => Idle,
            nameof(Ready)     => Ready,
            nameof(Running)   => Running,
            nameof(Completed) => Completed,
            nameof(Failed)    => Failed,
            nameof(Paused)    => Paused,
            nameof(Cancelled) => Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, null)
        };
    }
}