namespace Magnett.Automation.Core.Commons;

public abstract record Enumeration(int Id, string Name)
{
    public int Id { get; private init; } = Id;
    public string Name { get; private init; } = Name;
}