namespace Magnett.Automation.Core.Contexts;

/// <summary>
/// This interface represents a vault that can store and retrieve values of heterogeneous types.
/// The library provides a default implementation of this interface ContextVault <see cref="ContextVault"/>.
/// </summary>
public interface IContextVault : IDictionaryWrapper<object>;