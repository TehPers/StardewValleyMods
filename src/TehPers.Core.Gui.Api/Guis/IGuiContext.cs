using TehPers.Core.Gui.Api.Components;

namespace TehPers.Core.Gui.Api.Guis;

/// <summary>
/// A context for a GUI.
/// </summary>
public interface IGuiContext : IGuiBuilder
{
    /// <summary>
    /// Overlays a component on top of other components in the GUI.
    /// </summary>
    /// <param name="component">The component to add as an overlay.</param>
    void AddOverlay(IGuiComponent component);

    /// <summary>
    /// Removes an overlay component.
    /// </summary>
    /// <param name="component">The overlay component to remove.</param>
    void RemoveOverlay(IGuiComponent component);
}

/// <summary>
/// A context for a GUI.
/// </summary>
/// <typeparam name="TMessage">The type of message the GUI can accept.</typeparam>
public interface IGuiContext<in TMessage> : IGuiBuilder
{
    /// <summary>
    /// Sends a message to the GUI.
    /// </summary>
    /// <param name="message">The message to send.</param>
    void Update(TMessage message);
}
