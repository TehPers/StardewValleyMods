﻿using Microsoft.Xna.Framework;
using System;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Horizontally aligns a component. This removes any maximum width constraint.
    /// </summary>
    /// <param name="Inner">The inner component.</param>
    /// <param name="Alignment">The type of alignment to apply.</param>
    public record HorizontalAlign(
        IGuiComponent Inner,
        HorizontalAlignment Alignment
    ) : IGuiComponent
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            var innerConstraints = this.Inner.GetConstraints();
            return innerConstraints with
            {
                MaxSize = innerConstraints.MaxSize with
                {
                    Width = null,
                },
            };
        }

        /// <inheritdoc />
        public void Handle(GuiEvent e, Rectangle bounds)
        {
            this.Inner.Handle(e, this.GetInnerBounds(bounds));
        }

        private Rectangle GetInnerBounds(Rectangle bounds)
        {
            // Calculate inner width
            var innerConstraints = this.Inner.GetConstraints();
            var innerWidth = innerConstraints.MaxSize.Width switch
            {
                null => bounds.Width,
                { } maxWidth => (int)Math.Ceiling(Math.Min(maxWidth, bounds.Width)),
            };

            // Calculate x position
            var x = this.Alignment switch
            {
                HorizontalAlignment.Left => bounds.Left,
                HorizontalAlignment.Center => bounds.Left + (bounds.Width - innerWidth) / 2,
                HorizontalAlignment.Right => bounds.Right - innerWidth,
                _ => throw new InvalidOperationException(
                    $"{nameof(this.Alignment)} has an invalid value"
                ),
            };

            // Layout inner component
            return new(x, bounds.Y, innerWidth, bounds.Height);
        }
    }
}