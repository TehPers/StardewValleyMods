using Microsoft.Xna.Framework;
using TehPers.Core.Gui.Api.Guis;

namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A GUI component that can be drawn to the screen.
/// </summary>
public interface IGuiComponent
{
    /// <summary>
    /// Gets the GUI builder.
    /// </summary>
    public IGuiBuilder GuiBuilder { get; }

    /// <summary>
    /// Gets the constraints on how this component should be rendered.
    /// </summary>
    /// <returns>The constraints on how this component should be rendered.</returns>
    IGuiConstraints GetConstraints();

    /// <summary>
    /// Handles a UI event. This is where the component draws to the screen, handles input, and
    /// does whatever else it needs to do.
    /// </summary>
    /// <param name="e">The event data.</param>
    /// <param name="bounds">The bounds of the component.</param>
    /// <returns>Whether this component was updated.</returns>
    void Handle(IGuiEvent e, Rectangle bounds);
}
