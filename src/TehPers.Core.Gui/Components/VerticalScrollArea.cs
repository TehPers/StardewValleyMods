using Microsoft.Xna.Framework;
using System;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Extensions;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="IVerticalScrollArea" />
internal record VerticalScrollArea(
    IGuiBuilder Builder,
    IGuiComponent Inner,
    IVerticalScrollbar.IState State
) : BaseGuiComponent(Builder), IVerticalScrollArea
{
    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        var areaConstraints = this.Inner.GetConstraints();
        return new GuiConstraints(MinSize: new GuiSize(areaConstraints.MinSize.Width, 0));
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        // Get inner constraints
        var innerConstraints = this.Inner.GetConstraints();
        var innerHeight = (int)Math.Ceiling(innerConstraints.MinSize.Height);

        // Update scrollbar state
        this.State.MinValue = 0;
        this.State.MaxValue = innerHeight - bounds.Height;

        // Render inner component
        var offsetY = this.State.Value;
        var innerBounds = new Rectangle(bounds.X, bounds.Y - offsetY, bounds.Width, innerHeight);

        if (e .IsDraw(out var batch))
        {
            batch.WithScissorRect(
                bounds,
                batch => this.Inner.Handle(new GuiEvent.Draw(batch), innerBounds)
            );
        }
        else if (e.IsHover(out var hoverPos))
        {
            if (bounds.Contains(hoverPos))
            {
                this.Inner.Handle(e, innerBounds);
            }
        }
        else if (e.IsReceiveClick(out var clickPos, out _))
        {
            if (bounds.Contains(clickPos))
            {
                this.Inner.Handle(e, innerBounds);
            }
        }
        else if (e.IsScroll(out var direction))
        {
            this.State.Value -= 5 * direction / 120;
            this.Inner.Handle(e, innerBounds);
        }
        else
        {
            this.Inner.Handle(e, innerBounds);
        }
    }

    /// <inheritdoc />
    public IVerticalScrollArea WithInner(IGuiComponent inner)
    {
        return this with {Inner = inner};
    }

    /// <inheritdoc />
    public IVerticalScrollArea WithState(IVerticalScrollbar.IState state)
    {
        return this with {State = state};
    }
}
