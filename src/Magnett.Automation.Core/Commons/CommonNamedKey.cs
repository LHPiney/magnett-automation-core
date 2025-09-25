using System.Diagnostics;

namespace Magnett.Automation.Core.Commons;

/// <summary>
/// Represents a strongly-typed key with name and scope for identifying entities.
/// Provides value-based equality and immutability.
/// </summary>
[DebuggerDisplay("{Name}.{Scope}")]
public record CommonNamedKey
{
    /// <summary>
    /// The key name.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// The key scope (default is "default").
    /// </summary>
    public string Scope { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="CommonNamedKey"/>.
    /// </summary>
    /// <param name="name">The key name.</param>
    /// <param name="scope">The key scope (optional).</param>
    /// <exception cref="ArgumentNullException">Thrown if the name is null or empty.</exception>
    protected CommonNamedKey(string name, string scope = "default")
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));
        
        Name = name;
        Scope = scope;
    }

    /// <summary>
    /// Implicitly converts a string to a <see cref="CommonNamedKey"/> with default scope.
    /// </summary>
    /// <param name="name">The key name.</param>
    public static implicit operator CommonNamedKey(string name)
    {
        return new CommonNamedKey(name);
    }

    /// <summary>
    /// Compares the key with an <see cref="Enumeration"/> instance by name.
    /// </summary>
    /// <param name="other">Enumeration to compare.</param>
    /// <returns>True if the name matches; otherwise, false.</returns>
    public bool Equals(Enumeration other)
    {
        if (other is null)
            return false;

        return other.Name == Name;
    }

    /// <summary>
    /// Returns the string representation of the key (name.scope).
    /// </summary>
    /// <returns>String in the format "Name.Scope".</returns>
    public override string ToString()
    {
        return $"{Name}.{Scope}";
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommonNamedKey"/>.
    /// </summary>
    /// <param name="name">The key name.</param>
    /// <param name="scope">The key scope (optional).</param>
    /// <returns>An instance of <see cref="CommonNamedKey"/>.</returns>
    public static CommonNamedKey Create(string name, string scope = "default")
    {
        return new CommonNamedKey(name, scope);
    }
}