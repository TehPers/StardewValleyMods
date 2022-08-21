using Microsoft.Xna.Framework;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;

namespace TehPers.Core.Gui.Components;

/// <summary>
/// Empty space in a GUI.
/// </summary>
internal record EmptySpace(IGuiBuilder Builder) : BaseGuiComponent(Builder)
{
    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        return new GuiConstraints();
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
    }
}
