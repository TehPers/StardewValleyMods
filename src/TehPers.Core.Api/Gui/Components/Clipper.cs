using Microsoft.Xna.Framework;
using System;
using TehPers.Core.Api.Extensions;

namespace TehPers.Core.Api.Gui.Components
{
    /// <summary>
    /// Clips this component, removing its minimum size constraint. This constrains its rendering
    /// area and mouse inputs if it is shrunk.
    /// </summary>
    internal record Clipper : ComponentWrapper
    {
        public override IGuiComponent Inner { get; }

        /// <summary>
        /// Creates a new clipped component.
        /// </summary>
        /// <param name="inner">The component to clip.</param>
        public Clipper(IGuiComponent inner)
        {
            this.Inner = inner;
        }

        /// <inheritdoc />
        public override GuiConstraints GetConstraints()
        {
            return base.GetConstraints() with
            {
                MinSize = GuiSize.Zero,
            };
        }

        /// <inheritdoc />
        public override void Handle(GuiEvent e, Rectangle bounds)
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
            switch (e)
            {
                case GuiEvent.ReceiveClick(var position, _) when !bounds.Contains(position):
                    break;
                case GuiEvent.Draw(var batch) when bounds.Size != Point.Zero:
                    batch.WithScissorRect(
                        bounds,
                        batch => base.Handle(new GuiEvent.Draw(batch), innerBounds)
                    );
                    break;
                default:
                    base.Handle(e, innerBounds);
                    break;
            }
        }
    }
}
