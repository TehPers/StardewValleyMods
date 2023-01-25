namespace TehPers.Core.Gui.Api.Components.Layouts;

/// <summary>
/// A builder for a GUI layout.
/// </summary>
public interface ILayoutBuilder
{
    /// <summary>
    /// Adds a new component to this layout.
    /// </summary>
    /// <param name="component">The component to add to the layout.</param>
    void Add(IGuiComponent component);
}
