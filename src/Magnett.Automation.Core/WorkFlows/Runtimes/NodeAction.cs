namespace Magnett.Automation.Core.WorkFlows.Runtimes;

public record NodeAction : Enumeration
{
    public static readonly NodeAction Init     = new(1, nameof(Init));
    public static readonly NodeAction Execute  = new(2, nameof(Execute));
    public static readonly NodeAction Pause    = new(3, nameof(Pause));
    public static readonly NodeAction Resume   = new(4, nameof(Resume));
    public static readonly NodeAction Cancel   = new(5, nameof(Cancel));
    public static readonly NodeAction Complete = new(6, nameof(Complete));
    public static readonly NodeAction Fail     = new(7, nameof(Fail));

    private NodeAction(int id, string name) : base(id, name)
    {
    }
}