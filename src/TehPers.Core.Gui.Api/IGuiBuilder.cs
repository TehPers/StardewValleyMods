namespace TehPers.Core.Gui.Api;

/// <summary>
/// A builder for creating GUI components.
/// </summary>
public interface IGuiBuilder
{
    /// <summary>
    /// Tries to add an extension. Extensions are visible to all GUI builders.
    /// </summary>
    /// <param name="key">The globally unique key for the extension.</param>
    /// <param name="extension">The extension to add.</param>
    /// <returns>Whether the extension was able to be added.</returns>
    bool TryAddExtension(string key, object extension);

    /// <summary>
    /// Tries to get an extension.
    /// </summary>
    /// <param name="key">The globally unique key for the extension.</param>
    /// <returns>Whether the extension was able to be retrieved.</returns>
    object? TryGetExtension(string key);
}
