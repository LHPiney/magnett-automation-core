namespace Magnett.Automation.Core.Common
{
    public abstract class Enumeration
    {
        public int Id { get; init; }
        public string Name { get; init; }

        public Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}