namespace Magnett.Automation.Core.Commons;

public interface IDictionaryWrapper<TItem>
{
    int Count { get; }
    void Add(CommonNamedKey key, TItem item);
    void Set(CommonNamedKey key, TItem item);
    TItem Get(CommonNamedKey key);
    bool HasItem(CommonNamedKey key);
    bool IsEmpty();
}