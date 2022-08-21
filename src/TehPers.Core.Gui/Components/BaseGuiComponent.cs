using Microsoft.Xna.Framework;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;

namespace TehPers.Core.Gui.Components;

internal abstract record BaseGuiComponent(IGuiBuilder GuiBuilder) : IGuiComponent
{
    /// <inheritdoc />
    public abstract IGuiConstraints GetConstraints();

    /// <inheritdoc />
    public abstract void Handle(IGuiEvent e, Rectangle bounds);
}
