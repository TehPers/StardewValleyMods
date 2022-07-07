using Microsoft.Xna.Framework;
using System;

namespace TehPers.Core.Api.Gui.Components
{
    /// <summary>
    /// Vertically aligns a component. This removes any maximum height constraint.
    /// </summary>
    internal record VerticalAligner : ComponentWrapper
    {
        private readonly VerticalAlignment alignment;

        public override IGuiComponent Inner { get; }

        /// <summary>
        /// Vertically aligns a component. This removes any maximum height constraint.
        /// </summary>
        /// <param name="inner">The inner component.</param>
        /// <param name="alignment">The type of alignment to apply.</param>
        public VerticalAligner(IGuiComponent inner, VerticalAlignment alignment)
        {
            this.Inner = inner;
            this.alignment = alignment;
        }

        /// <inheritdoc />
        public override GuiConstraints GetConstraints()
        {
            var innerConstraints = base.GetConstraints();
            return innerConstraints with
            {
                MaxSize = innerConstraints.MaxSize with
                {
                    Height = null,
                },
            };
        }

        /// <inheritdoc />
        public override void Handle(GuiEvent e, Rectangle bounds)
        {
            base.Handle(e, this.GetInnerBounds(bounds));
        }

        private Rectangle GetInnerBounds(Rectangle bounds)
        {
            // Calculate inner width
            var innerConstraints = base.GetConstraints();
            // Calculate inner width
            var innerHeight = innerConstraints.MaxSize.Height switch
            {
                null => bounds.Height,
                { } maxHeight => (int)Math.Ceiling(Math.Min(maxHeight, bounds.Height)),
            };

            // Calculate y position
            var y = this.alignment switch
            {
                VerticalAlignment.Top => bounds.Top,
                VerticalAlignment.Center => bounds.Top + (bounds.Height - innerHeight) / 2,
                VerticalAlignment.Bottom => bounds.Bottom - innerHeight,
                _ => throw new InvalidOperationException(
                    $"{nameof(this.alignment)} has an invalid value"
                ),
            };

            // Layout inner component
            return new(bounds.X, y, bounds.Width, innerHeight);
        }
    }
}
