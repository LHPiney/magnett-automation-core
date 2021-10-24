using System;
using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.WorkFlows.Implementations
{
    public class NodeResponse : CommonNamedKey
    {
        protected NodeResponse(string nodeName, string responseCode) : 
            base($"{nodeName}.{responseCode}")
        {
            if (string.IsNullOrEmpty(nodeName))
                throw new ArgumentNullException(nameof(nodeName));

            if (string.IsNullOrEmpty(responseCode))
                throw new ArgumentNullException(nameof(responseCode));

            NodeName = nodeName;
            ResponseCode = responseCode;
        }

        public string NodeName { get; private set; }
        public string ResponseCode { get; private set; }

        public static NodeResponse Create(string nodeName, string responseCode)
        {
            return new NodeResponse(nodeName, responseCode);
        }
    }
}