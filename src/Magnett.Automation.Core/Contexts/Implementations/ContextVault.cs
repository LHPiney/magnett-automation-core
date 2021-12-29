using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.Contexts.Implementations
{
    internal class ContextVault : DictionaryWrapper<object>, IContextVault
    {
        public static ContextVault Create()
        {
            return new();
        }
    }
}