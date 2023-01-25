namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A partially defined size for use in a GUI.
/// </summary>
public interface IPartialGuiSize
{
    /// <summary>
    /// The width of the component, if any.
    /// </summary>
    float? Width { get; }

    /// <summary>
    /// The height of the component, if any.
    /// </summary>
    float? Height { get; }

    /// <summary>
    /// Deconstructs this <see cref="IPartialGuiSize"/>.
    /// </summary>
    /// <param name="width">The width of the component, if any.</param>
    /// <param name="height">The height of the component, if any.</param>
    void Deconstruct(out float? width, out float? height);
}
