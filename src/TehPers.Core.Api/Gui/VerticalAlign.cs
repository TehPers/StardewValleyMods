﻿using Microsoft.Xna.Framework;
using System;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Vertically aligns a component. This removes any maximum height constraint.
    /// </summary>
    /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
    /// <param name="Inner">The inner component.</param>
    /// <param name="Alignment">The type of alignment to apply.</param>
    public record VerticalAlign<TResponse>(
        IGuiComponent<TResponse> Inner,
        VerticalAlignment Alignment
    ) : IGuiComponent<TResponse>
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            var innerConstraints = this.Inner.GetConstraints();
            return innerConstraints with
            {
                MaxSize = innerConstraints.MaxSize with
                {
                    Height = null,
                },
            };
        }

        /// <inheritdoc />
        public TResponse Handle(GuiEvent e, Rectangle bounds)
        {
            return this.Inner.Handle(e, this.GetInnerBounds(bounds));
        }

        private Rectangle GetInnerBounds(Rectangle bounds)
        {
            // Calculate inner width
            var innerConstraints = this.Inner.GetConstraints();
            // Calculate inner width
            var innerHeight = innerConstraints.MaxSize.Height switch
            {
                null => bounds.Height,
                { } maxHeight => (int)Math.Ceiling(Math.Min(maxHeight, bounds.Height)),
            };

            // Calculate y position
            var y = this.Alignment switch
            {
                VerticalAlignment.Top => bounds.Top,
                VerticalAlignment.Center => bounds.Top + (bounds.Height - innerHeight) / 2,
                VerticalAlignment.Bottom => bounds.Bottom - innerHeight,
                _ => throw new InvalidOperationException(
                    $"{nameof(this.Alignment)} has an invalid value"
                ),
            };

            // Layout inner component
            return new(bounds.X, y, bounds.Width, innerHeight);
        }
    }
}
