using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.SimpleMachine.Definitions;

public record Action : Enumeration
{
    public static readonly Action Start    = new(1, "Start");
    public static readonly Action Pause    = new(2, "Pause");
    public static readonly Action Continue = new(3, "Continue");
    public static readonly Action Finish   = new(4, "Finish");
            
    private Action(int id, string name) : base(id, name)
    {
            
    }
}