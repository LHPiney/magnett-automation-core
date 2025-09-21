namespace Magnett.Automation.Core.Contexts.Implementations;

internal class ContextVault : DictionaryWrapper<object>, IContextVault
{
    /// <summary>
    /// Creates a new ContextVault instance.
    /// </summary>
    /// <returns>A new ContextVault instance.</returns>
    public static ContextVault Create()
    {
        return new ContextVault();
    }
}