namespace Magnett.Automation.Core.Commons;

/// <summary>
/// Defines an interface for a strongly-typed dictionary wrapper using <see cref="CommonNamedKey"/> as key.
/// </summary>
/// <typeparam name="TItem">Type of the items stored in the dictionary.</typeparam>
public interface IDictionaryWrapper<TItem>
{
    /// <summary>
    /// Gets the number of items stored.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Adds an item to the dictionary with the specified key.
    /// </summary>
    /// <param name="key">Key of type <see cref="CommonNamedKey"/>.</param>
    /// <param name="item">Item to add.</param>
    TItem Add(CommonNamedKey key, TItem item);

    /// <summary>
    /// Sets or replaces the item associated with the specified key.
    /// </summary>
    /// <param name="key">Key of type <see cref="CommonNamedKey"/>.</param>
    /// <param name="item">Item to set.</param>
    TItem Set(CommonNamedKey key, TItem item);

    /// <summary>
    /// Gets the item associated with the specified key.
    /// </summary>
    /// <param name="key">Key of type <see cref="CommonNamedKey"/>.</param>
    /// <returns>The stored item.</returns>
    TItem Get(CommonNamedKey key);

    /// <summary>
    /// Indicates whether there is an item associated with the specified key.
    /// </summary>
    /// <param name="key">Key of type <see cref="CommonNamedKey"/>.</param>
    /// <returns>True if the item exists; otherwise, false.</returns>
    bool HasItem(CommonNamedKey key);

    /// <summary>
    /// Gets an enumeration of all stored keys.
    /// </summary>
    /// <returns>Enumeration of <see cref="CommonNamedKey"/>.</returns>
    IEnumerable<CommonNamedKey> GetKeys();

    /// <summary>
    /// Removes the element with the specified key from the collection.
    /// </summary>
    /// <param name="key">The key of the element to remove. Cannot be null.</param>
    /// <returns>true if the element is successfully removed; otherwise, false. This method also returns false if the key was not
    /// found in the collection.</returns>
    bool Remove(CommonNamedKey key);

    /// <summary>
    /// Gets an enumeration of all stored values.
    /// </summary>
    /// <returns>Enumeration of items of type <typeparamref name="TItem"/>.</returns>
    IEnumerable<TItem> GetValues();

    /// <summary>
    /// Indicates whether the dictionary is empty.
    /// </summary>
    /// <returns>True if it is empty; otherwise, false.</returns>
    bool IsEmpty();
}