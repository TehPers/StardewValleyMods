using Microsoft.Xna.Framework;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Guis;

namespace TehPers.Core.Gui.Components;

internal abstract record ComponentWrapper
    (IGuiBuilder Builder, IGuiComponent Inner) : BaseGuiComponent(Builder)
{
    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        return this.Inner.GetConstraints();
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        this.Inner.Handle(e, bounds);
    }
}
