using System;
using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.WorkFlows.Implementations
{
    public class NodeLink : INodeLink
    {
        public CommonNamedKey Key { get; }
        public CommonNamedKey FromNodeKey { get; }
        public CommonNamedKey ToNodeKey { get; }
        public string Code { get; }

        private NodeLink(
            CommonNamedKey fromNodeKey,
            CommonNamedKey toNodeKey,
            string code)
        {
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException(nameof(code));
            
            FromNodeKey = fromNodeKey
                          ?? throw new ArgumentNullException(nameof(fromNodeKey));
            ToNodeKey = toNodeKey
                        ?? throw new ArgumentNullException(nameof(toNodeKey));
            Code = code;
            
            //Generate Unique key
            Key = CommonNamedKey.Create($"{FromNodeKey.Name}.{Code}");
        }

        public static INodeLink Create(
            CommonNamedKey fromNodeKey,
            CommonNamedKey toNodeKey,
            string code)
        {
            return new NodeLink(fromNodeKey, toNodeKey, code);
        }
    }
}