using TehPers.Core.Gui.Api.Internal;

namespace TehPers.Core.Gui.Api;

/// <summary>
/// An interface for interacting with TehPers.Core.Gui.
/// </summary>
public interface ICoreGuiApi
{
    /// <summary>
    /// Gets the GUI builder.
    /// </summary>
    IGuiBuilder GuiBuilder { get; }

    /// <summary>
    /// Internal use APIs.
    /// </summary>
    IInternalApi InternalApi { get; }
}
