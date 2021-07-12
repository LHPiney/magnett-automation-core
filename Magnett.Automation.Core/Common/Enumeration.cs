namespace Magnett.Automation.Core.Common
{
    public abstract class Enumeration
    {
        private int Id { get; init; }
        public string Name { get; init; }

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}