using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;

namespace Magnett.Automation.Core.WorkFlows.Implementations
{
    public abstract class NodeBase
    {
        protected Context GlobalContext { get; private set; }

        public CommonNamedKey Key { get; }
        
        protected NodeBase(string name) : this(CommonNamedKey.Create(name))
        {
            
        }

        protected NodeBase(CommonNamedKey key)
        {
            Key = key 
                  ?? throw new ArgumentNullException(nameof(key));
        }

        public virtual void Init(Context globalContext)
        {
            GlobalContext = globalContext
                           ?? throw new ArgumentNullException(nameof(globalContext));
        }
    }
}