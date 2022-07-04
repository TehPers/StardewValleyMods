using Microsoft.Xna.Framework;
using System;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Adds padding to a component.
    /// </summary>
    /// <param name="Inner">The inner component.</param>
    /// <param name="Left">Padding to add to the left side.</param>
    /// <param name="Right">Padding to add to the right side.</param>
    /// <param name="Top">Padding to add to the top.</param>
    /// <param name="Bottom">Padding to add to the bottom.</param>
    public record PaddedComponent(
        IGuiComponent Inner,
        float Left,
        float Right,
        float Top,
        float Bottom
    ) : IGuiComponent
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            var innerConstraints = this.Inner.GetConstraints();
            return innerConstraints with
            {
                MinSize = new(
                    innerConstraints.MinSize.Width + this.Left + this.Right,
                    innerConstraints.MinSize.Height + this.Top + this.Bottom
                ),
                MaxSize = new(
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
                ),
            };
        }

        /// <inheritdoc />
        public void Handle(GuiEvent e, Rectangle bounds)
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
    }
}
