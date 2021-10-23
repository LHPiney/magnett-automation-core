using System;

namespace Magnett.Automation.Core.Commons
{
    public class CommonNamedKey
    {
        public string Name { get; }

        protected CommonNamedKey(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        public bool Equals(string str)
        {
            return !string.IsNullOrEmpty(str) && Name.Equals(str);
        }
        
        public bool Equals(Enumeration enumeration)
        {
            return enumeration != null && Name.Equals(enumeration.Name);
        }
        
        public override bool Equals(object obj)
        {
            return obj is CommonNamedKey key && Name.Equals(key.Name);
        }

        protected bool Equals(CommonNamedKey other)
        {
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static CommonNamedKey Create(string name)
        {
            return new CommonNamedKey(name);
        }
    }
}