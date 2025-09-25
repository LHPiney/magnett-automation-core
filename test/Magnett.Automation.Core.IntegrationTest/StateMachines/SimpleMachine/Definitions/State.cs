using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.SimpleMachine.Definitions;

public record State : Enumeration
{
    public static readonly State Init     = new(1, "Init");
    public static readonly State Working  = new(2, "Working");
    public static readonly State Paused   = new(3, "Paused");
    public static readonly State Finished = new(4, "Finished");
        
    private State(int id, string name) : base(id, name)
    {
            
    }
}