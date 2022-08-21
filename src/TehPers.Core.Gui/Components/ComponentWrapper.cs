using Microsoft.Xna.Framework;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;

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
