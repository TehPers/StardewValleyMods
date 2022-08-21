using Microsoft.Xna.Framework;
using System;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="IAligner" />
internal record Aligner(
    IGuiBuilder Builder,
    IGuiComponent Inner,
    HorizontalAlignment Horizontal,
    VerticalAlignment Vertical
) : ComponentWrapper(Builder, Inner), IAligner
{
    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        var innerConstraints = base.GetConstraints();
        var maxSize = new PartialGuiSize(
            this.Horizontal is HorizontalAlignment.None ? innerConstraints.MaxSize.Width : null,
            this.Vertical is VerticalAlignment.None ? innerConstraints.MaxSize.Height : null
        );
        return new GuiConstraints(innerConstraints.MinSize, maxSize);
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        base.Handle(e, this.GetInnerBounds(bounds));
    }

    private Rectangle GetInnerBounds(Rectangle bounds)
    {
        var innerConstraints = base.GetConstraints();

        // Calculate inner width
        var innerWidth = innerConstraints.MaxSize.Width switch
        {
            null => bounds.Width,
            _ when this.Horizontal is HorizontalAlignment.None => bounds.Width,
            { } maxWidth => (int)Math.Ceiling(Math.Min(maxWidth, bounds.Width)),
        };

        // Calculate inner height
        var innerHeight = innerConstraints.MaxSize.Height switch
        {
            null => bounds.Height,
            _ when this.Vertical is VerticalAlignment.None => bounds.Height,
            { } maxHeight => (int)Math.Ceiling(Math.Min(maxHeight, bounds.Height)),
        };

        // Calculate x position
        var innerX = this.Horizontal switch
        {
            HorizontalAlignment.None or HorizontalAlignment.Left => bounds.Left,
            HorizontalAlignment.Center => bounds.Left + (bounds.Width - innerWidth) / 2,
            HorizontalAlignment.Right => bounds.Right - innerWidth,
            _ => throw new InvalidOperationException(
                $"Invalid horizontal alignment: {this.Horizontal}"
            ),
        };


        // Calculate y position
        var innerY = this.Vertical switch
        {
            VerticalAlignment.None or VerticalAlignment.Top => bounds.Top,
            VerticalAlignment.Center => bounds.Top + (bounds.Height - innerHeight) / 2,
            VerticalAlignment.Bottom => bounds.Bottom - innerHeight,
            _ => throw new InvalidOperationException(
                $"Invalid vertical alignment: {this.Vertical}"
            ),
        };


        // Layout inner component
        return new(innerX, innerY, innerWidth, innerHeight);
    }

    /// <inheritdoc />
    public IAligner WithInner(IGuiComponent inner)
    {
        return this with {Inner = inner};
    }

    /// <inheritdoc />
    public IAligner WithHorizontalAlignment(HorizontalAlignment alignment)
    {
        return this with {Horizontal = alignment};
    }

    /// <inheritdoc />
    public IAligner WithVerticalAlignment(VerticalAlignment alignment)
    {
        return this with {Vertical = alignment};
    }
}
