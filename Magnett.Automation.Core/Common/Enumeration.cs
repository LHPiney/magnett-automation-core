namespace Magnett.Automation.Core.Common
{
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
}