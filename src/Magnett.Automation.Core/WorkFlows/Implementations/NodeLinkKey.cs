using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Commons;

[assembly: InternalsVisibleTo("Magnett.Automation.Core.UnitTest")]
namespace Magnett.Automation.Core.WorkFlows.Implementations
{
    internal class NodeLinkKey : CommonNamedKey 
    {
        private NodeLinkKey(string name) : base(name)
        {
            
        }

        private NodeLinkKey(CommonNamedKey node, string code) : this($"{node?.Name}.{code}")
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException(nameof(code));
        }

        public static NodeLinkKey Create(CommonNamedKey node, string code)
        {
            return new NodeLinkKey(node, code);
        }
    }
}