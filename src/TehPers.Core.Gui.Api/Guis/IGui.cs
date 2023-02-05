using TehPers.Core.Gui.Api.Components;

namespace TehPers.Core.Gui.Api.Guis;

/// <summary>
/// A graphical user interface.
/// </summary>
/// <typeparam name="TMessage">The kind of message that can be generated from this GUI's view.</typeparam>
public interface IGui<TMessage>
{
    /// <summary>
    /// Whether this GUI needs to capture input.
    /// </summary>
    public bool CaptureInput { get; }

    /// <summary>
    /// Updates this GUI with a message received from its view in response to some event.
    /// </summary>
    /// <param name="message">The message being sent from this GUI's view.</param>
    public void Update(TMessage message);

    /// <summary>
    /// Creates the root component for this GUI.
    /// </summary>
    /// <param name="ctx">The context for the GUI. This can be used to create components.</param>
    /// <returns>The root component.</returns>
    public IGuiComponent View(IGuiContext<TMessage> ctx);
}
