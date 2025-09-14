using System.Collections.Concurrent;
using System.Linq;

namespace Magnett.Automation.Core.Commons;

/// <summary>
/// Abstract base implementation of <see cref="IDictionaryWrapper{TItem}"/> using <see cref="CommonNamedKey"/> as key.
/// This implementation is thread-safe by default, as it uses <see cref="ConcurrentDictionary{TKey, TValue}"/> internally.
/// </summary>
/// <typeparam name="TItem">Type of the items stored.</typeparam>
public abstract class DictionaryWrapper<TItem> : IDictionaryWrapper<TItem>
{
    private const int Zero = 0;
            
    private readonly ConcurrentDictionary<CommonNamedKey, TItem> _dictionary;
        
    /// <inheritdoc/>
    public int Count => _dictionary.Count;

    /// <summary>
    /// Initializes a new instance of <see cref="DictionaryWrapper{TItem}"/>.
    /// </summary>
    protected DictionaryWrapper()
    {
        _dictionary = new ConcurrentDictionary<CommonNamedKey, TItem>();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DictionaryWrapper{TItem}"/> with an existing dictionary.
    /// </summary>
    /// <param name="dictionary">Base dictionary to use. If not a <see cref="ConcurrentDictionary{TKey, TValue}"/>, its contents will be copied to a new concurrent dictionary.</param>
    /// <exception cref="ArgumentNullException">If the dictionary is null.</exception>
    protected DictionaryWrapper(IDictionary<CommonNamedKey, TItem> dictionary)
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        _dictionary = dictionary is ConcurrentDictionary<CommonNamedKey, TItem> concurrentDict
            ? concurrentDict
            : LoadVault(dictionary);

        static ConcurrentDictionary<CommonNamedKey, TItem> LoadVault(IDictionary<CommonNamedKey, TItem> dict)
        {
            var concurrentDict = new ConcurrentDictionary<CommonNamedKey, TItem>();
            foreach (var kvp in dict)
            {
                concurrentDict.TryAdd(kvp.Key, kvp.Value);
            }
            return concurrentDict;
        }
    }

    /// <inheritdoc/>
    public virtual TItem Add(CommonNamedKey key, TItem item)
    {
        return _dictionary.TryAdd(key, item) 
            ? item 
            : throw new ArgumentException($"An item with the same key has already been added: {key}", nameof(key));
    }

    /// <inheritdoc/>
    public virtual TItem Set(CommonNamedKey key, TItem item)
    {
        return _dictionary[key] = item;
    }

    /// <inheritdoc/>
    public virtual TItem Get(CommonNamedKey key)
    {
        return _dictionary.TryGetValue(key, out var item)
            ? item
            : throw new KeyNotFoundException($"The given key was not present in the dictionary: {key}");    
    }

    /// <inheritdoc/>
    public virtual bool Remove(CommonNamedKey key)
    {
        return _dictionary.TryRemove(key, out _);
    }

    /// <inheritdoc/>
    public virtual bool HasItem(CommonNamedKey key)
    {
        return _dictionary.ContainsKey(key);
    }
    
    /// <inheritdoc/>
    public virtual IEnumerable<CommonNamedKey> GetKeys()
    {
        return _dictionary.Keys.AsEnumerable();
    }

    /// <inheritdoc/>
    public virtual IEnumerable<TItem> GetValues()
    {
        return _dictionary.Values.AsEnumerable();
    }

    /// <inheritdoc/>
    public virtual bool IsEmpty()
    {
        return _dictionary.IsEmpty;
    }
}