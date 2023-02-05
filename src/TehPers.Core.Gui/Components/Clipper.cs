using Microsoft.Xna.Framework;
using System;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Guis;
using TehPers.Core.Gui.Extensions;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="IClipper"/>
internal record Clipper
    (IGuiBuilder Builder, IGuiComponent Inner) : ComponentWrapper(Builder, Inner), IClipper
{
    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        var innerConstraints = base.GetConstraints();
        return new GuiConstraints(GuiSize.Zero, innerConstraints.MaxSize);
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        var guiConstraints = this.Inner.GetConstraints();
        var innerWidth = guiConstraints.MaxSize.Width is { } maxWidth
            ? Math.Clamp(bounds.Width, guiConstraints.MinSize.Width, maxWidth)
            : guiConstraints.MinSize.Width;
        var innerHeight = guiConstraints.MaxSize.Height is { } maxHeight
            ? Math.Clamp(bounds.Height, guiConstraints.MinSize.Height, maxHeight)
            : guiConstraints.MinSize.Height;
        var innerBounds = new Rectangle(
            bounds.X,
            bounds.Y,
            (int)Math.Ceiling(innerWidth),
            (int)Math.Ceiling(innerHeight)
        );

        if (e.IsReceiveClick(out var position, out _) && !bounds.Contains(position))
        {
            // Do nothing
        }
        else if (e.IsDraw(out var batch) && bounds.Size != Point.Zero)
        {
            batch.WithScissorRect(
                bounds,
                batch => base.Handle(new GuiEvent.Draw(batch), innerBounds)
            );
        }
        else
        {
            base.Handle(e, innerBounds);
        }
    }

    /// <inheritdoc />
    public IClipper WithInner(IGuiComponent inner)
    {
        return this with {Inner = inner};
    }
}
