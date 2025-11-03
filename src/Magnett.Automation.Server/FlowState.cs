using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Server;

public record FlowState : Enumeration
{
    private const int RunningId = 1;
    private const int PausedId = 2;
    private const int CompletedId = 3;
    private const int FailedId = 4;
    
    public static readonly FlowState Running = new(RunningId, nameof(Running));
    public static readonly FlowState Paused = new(PausedId, nameof(Paused));
    public static readonly FlowState Completed = new(CompletedId, nameof(Completed));
    public static readonly FlowState Failed = new(FailedId, nameof(Failed));

    private FlowState(int id, string name) : base(id, name)
    {
    }
}

