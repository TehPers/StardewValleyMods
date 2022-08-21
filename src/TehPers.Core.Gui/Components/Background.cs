using Microsoft.Xna.Framework;
using System;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="IBackground"/>
internal record Background(
    IGuiBuilder Builder,
    IGuiComponent Inner,
    IGuiComponent BackgroundComponent
) : ComponentWrapper(Builder, Inner), IBackground
{
    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        var fgConstraints = this.Inner.GetConstraints();
        var bgConstraints = this.BackgroundComponent.GetConstraints();
        return new GuiConstraints()
        {
            MinSize = new GuiSize(
                Math.Max(bgConstraints.MinSize.Width, fgConstraints.MinSize.Width),
                Math.Max(bgConstraints.MinSize.Height, fgConstraints.MinSize.Height)
            ),
            MaxSize = new PartialGuiSize(
                (bgConstraints.MaxSize.Width, fgConstraints.MaxSize.Width) switch
                {
                    (null, var w) => w,
                    (var w, null) => w,
                    ({ } w1, { } w2) => Math.Min(w1, w2),
                },
                (bgConstraints.MaxSize.Height, fgConstraints.MaxSize.Height) switch
                {
                    (null, var h) => h,
                    (var h, null) => h,
                    ({ } h1, { } h2) => Math.Min(h1, h2),
                }
            ),
        };
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        this.BackgroundComponent.Handle(e, bounds);
        this.Inner.Handle(e, bounds);
    }

    /// <inheritdoc />
    public IBackground WithInner(IGuiComponent inner)
    {
        return this with {Inner = inner};
    }

    /// <inheritdoc />
    public IBackground WithBackground(IGuiComponent background)
    {
        return this with {BackgroundComponent = background};
    }
}
