using System;
    
namespace Magnett.Automation.Core.Common
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

        public static CommonNamedKey Create(string name)
        {
            return new CommonNamedKey(name);
        }
    }
}