using System;

namespace Magnett.Automation.Core.Contexts
{
    public class Context
    {
        private readonly IContextVault _contextVault;

        private Context(IContextVault contextVault)
        {
            _contextVault = contextVault
                            ?? throw new ArgumentNullException(nameof(contextVault));
        }
        
        public void Store<TValue>(ContextField<TValue> field, TValue value)
        {
            _contextVault.Set(field, value);
        }

        public TValue Value<TValue>(ContextField<TValue> field)
        {
            var result = default(TValue);

            if (_contextVault.HasItem(field))
            {
                result = (TValue) _contextVault.Get(field);
            }

            return result;
        }

        public static Context Create(IContextVault contextVault)
        {
            return new Context(contextVault);
        }
    }
}