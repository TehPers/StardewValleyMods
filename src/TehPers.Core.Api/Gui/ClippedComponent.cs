using Microsoft.Xna.Framework;
using System;
using TehPers.Core.Api.Extensions;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Clips this component, removing its minimum size constraint. This constrains its rendering
    /// area and mouse inputs if it is shrunk.
    /// </summary>
    internal record ClippedComponent : WrapperComponent
    {
        protected override IGuiComponent Inner { get; }

        /// <summary>
        /// Creates a new clipped component.
        /// </summary>
        /// <param name="inner">The component to clip.</param>
        public ClippedComponent(IGuiComponent inner)
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
            var innerBounds = new Rectangle(
                bounds.X,
                bounds.Y,
                (int)Math.Ceiling(guiConstraints.MinSize.Width),
                (int)Math.Ceiling(guiConstraints.MinSize.Height)
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
