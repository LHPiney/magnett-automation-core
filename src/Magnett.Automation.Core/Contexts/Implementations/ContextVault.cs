using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.Contexts
{
    public class ContextVault : DictionaryWrapper<object>, IContextVault
    {
        public static ContextVault Create()
        {
            return new();
        }
    }
}