namespace Magnett.Automation.Core.Commons;

public abstract class Enumeration
{
    protected int Id { get; private init; }
    public string Name { get; private init; }

    protected Enumeration(int id, string name)
    {
        Id = id;
        Name = name;
    }
}