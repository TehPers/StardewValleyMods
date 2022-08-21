using Microsoft.Xna.Framework;
using System;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="IComponentPadder"/>
internal record ComponentPadder(
    IGuiBuilder Builder,
    IGuiComponent Inner,
    float Left,
    float Right,
    float Top,
    float Bottom
) : ComponentWrapper(Builder, Inner), IComponentPadder
{
    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        var innerConstraints = this.Inner.GetConstraints();
        return new GuiConstraints(
            new GuiSize(
                innerConstraints.MinSize.Width + this.Left + this.Right,
                innerConstraints.MinSize.Height + this.Top + this.Bottom
            ),
            new PartialGuiSize(
                innerConstraints.MaxSize.Width switch
                {
                    null => null,
                    { } w => w + this.Left + this.Right
                },
                innerConstraints.MaxSize.Height switch
                {
                    null => null,
                    { } h => h + this.Top + this.Bottom
                }
            )
        );
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        this.Inner.Handle(e, this.GetInnerBounds(bounds));
    }

    private Rectangle GetInnerBounds(Rectangle bounds)
    {
        return new(
            (int)(bounds.X + this.Left),
            (int)(bounds.Y + this.Top),
            (int)Math.Max(0, Math.Ceiling(bounds.Width - this.Left - this.Right)),
            (int)Math.Max(0, Math.Ceiling(bounds.Height - this.Top - this.Bottom))
        );
    }

    /// <inheritdoc />
    public IComponentPadder WithInner(IGuiComponent inner)
    {
        return this with {Inner = inner};
    }

    /// <inheritdoc />
    public IComponentPadder WithLeft(float padding)
    {
        return this with {Left = padding};
    }

    /// <inheritdoc />
    public IComponentPadder WithRight(float padding)
    {
        return this with {Right = padding};
    }

    /// <inheritdoc />
    public IComponentPadder WithTop(float padding)
    {
        return this with {Top = padding};
    }

    /// <inheritdoc />
    public IComponentPadder WithBottom(float padding)
    {
        return this with {Bottom = padding};
    }
}
