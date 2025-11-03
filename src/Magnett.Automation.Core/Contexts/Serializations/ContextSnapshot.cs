namespace Magnett.Automation.Core.Contexts.Serializations;

public class ContextSnapshot
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ContextFieldSnapshot> Fields { get; set; } = new();
}