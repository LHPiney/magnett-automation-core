using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;

namespace Magnett.Automation.Core.WorkFlows.Implementations
{
    public abstract class NodeBase
    {
        protected Context GlobalContext { get; private set; }

        public CommonNamedKey Key { get; }
        
        protected NodeBase(string name)
        {
           Key = CommonNamedKey.Create(name);
        }

        public virtual void Init(Context globalContext)
        {
            GlobalContext = globalContext
                           ?? throw new ArgumentNullException(nameof(globalContext));
        }
    }
}