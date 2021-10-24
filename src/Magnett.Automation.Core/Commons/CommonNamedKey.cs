using System;
using System.Collections.Generic;

namespace Magnett.Automation.Core.Commons
{
    public class CommonNamedKey : IEqualityComparer<CommonNamedKey>
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
        
        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static CommonNamedKey Create(string name)
        {
            return new CommonNamedKey(name);
        }

        public bool Equals(CommonNamedKey x, CommonNamedKey y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
                
            return x.Name == y.Name;
        }

        public int GetHashCode(CommonNamedKey obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}