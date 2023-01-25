namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A size for use in a GUI.
/// </summary>
public interface IGuiSize
{
    /// <summary>
    /// The width of the component.
    /// </summary>
    float Width { get; }

    /// <summary>
    /// The height of the component.
    /// </summary>
    float Height { get; }

    /// <summary>
    /// Deconstructs this <see cref="IGuiSize"/>.
    /// </summary>
    /// <param name="width">The width of the component.</param>
    /// <param name="height">The height of the component.</param>
    void Deconstruct(out float width, out float height);
}
