using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.Contexts
{
    public class ContextField<TValue> : CommonNamedKey
    {
        private ContextField(string name) : base(name)
        {
        }

        public new static ContextField<TValue> Create(string fieldName)
        {
            return new ContextField<TValue>(fieldName);
        }
    }
}