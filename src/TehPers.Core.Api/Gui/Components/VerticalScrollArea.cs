using Microsoft.Xna.Framework;
using System;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Gui.States;

namespace TehPers.Core.Api.Gui.Components
{
    internal record VerticalScrollArea(IGuiComponent Inner, ScrollbarState State) : IGuiComponent
    {
        public GuiConstraints GetConstraints()
        {
            var areaConstraints = this.Inner.GetConstraints();
            return new()
            {
                MinSize = areaConstraints.MinSize with { Height = 0 },
            };
        }

        public void Handle(GuiEvent e, Rectangle bounds)
        {
            // Get inner constraints
            var innerConstraints = this.Inner.GetConstraints();
            var innerHeight = (int)Math.Ceiling(innerConstraints.MinSize.Height);

            // Update scrollbar state
            this.State.MinValue = 0;
            this.State.MaxValue = innerHeight - bounds.Height;

            // Render inner component
            var offsetY = this.State.Value;
            var innerBounds = new Rectangle(
                bounds.X,
                bounds.Y - offsetY,
                bounds.Width,
                innerHeight
            );
            switch (e)
            {
                case GuiEvent.Draw(var batch):
                    batch.WithScissorRect(
                        bounds,
                        batch => this.Inner.Handle(new GuiEvent.Draw(batch), innerBounds)
                    );
                    break;
                case GuiEvent.Hover(var position):
                    if (bounds.Contains(position))
                    {
                        this.Inner.Handle(e, bounds);
                    }

                    break;
                case GuiEvent.ReceiveClick(var position, _):
                    if (bounds.Contains(position))
                    {
                        this.Inner.Handle(e, bounds);
                    }

                    break;
                case GuiEvent.Scroll(var direction):
                    this.State.Value -= direction / 120;
                    this.Inner.Handle(e, innerBounds);
                    break;
                default:
                    this.Inner.Handle(e, innerBounds);
                    break;
            }
        }
    }
}
