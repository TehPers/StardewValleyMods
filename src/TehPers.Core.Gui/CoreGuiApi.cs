using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Guis;
using TehPers.Core.Gui.Api.Internal;

namespace TehPers.Core.Gui;

/// <inheritdoc cref="ICoreGuiApi" />
public class CoreGuiApi : ICoreGuiApi
{
    /// <inheritdoc />
    public IGuiBuilder GuiBuilder { get; }

    /// <inheritdoc />
    public IInternalApi InternalApi { get; }

    public CoreGuiApi()
    {
        this.GuiBuilder = new GuiBuilder();
        this.InternalApi = new InternalApi(this.GuiBuilder);
    }
}
