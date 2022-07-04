using Microsoft.Xna.Framework;
using System;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Horizontally aligns a component. This removes any maximum width constraint.
    /// </summary>
    internal record HorizontalAlignComponent : WrapperComponent
    {
        private readonly HorizontalAlignment alignment;

        public override IGuiComponent Inner { get; }

        /// <summary>
        /// Horizontally aligns a component. This removes any maximum width constraint.
        /// </summary>
        /// <param name="inner">The inner component.</param>
        /// <param name="alignment">The type of alignment to apply.</param>
        public HorizontalAlignComponent(IGuiComponent inner, HorizontalAlignment alignment)
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
                    Width = null,
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
            var innerWidth = innerConstraints.MaxSize.Width switch
            {
                null => bounds.Width,
                { } maxWidth => (int)Math.Ceiling(Math.Min(maxWidth, bounds.Width)),
            };

            // Calculate x position
            var x = this.alignment switch
            {
                HorizontalAlignment.Left => bounds.Left,
                HorizontalAlignment.Center => bounds.Left + (bounds.Width - innerWidth) / 2,
                HorizontalAlignment.Right => bounds.Right - innerWidth,
                _ => throw new InvalidOperationException(
                    $"{nameof(this.alignment)} has an invalid value"
                ),
            };

            // Layout inner component
            return new(x, bounds.Y, innerWidth, bounds.Height);
        }
    }
}
