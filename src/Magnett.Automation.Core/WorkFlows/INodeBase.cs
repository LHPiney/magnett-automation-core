﻿using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;

namespace Magnett.Automation.Core.WorkFlows
{
    public interface INodeBase  
    {
        public CommonNamedKey Key { get; }
        public bool IsInit { get; }

        public string Name => Key?.Name;

        void Init(Context flowContext);
    }
}