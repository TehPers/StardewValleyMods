using Microsoft.Xna.Framework;
using StardewValley;
using System;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Extensions.Drawing;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A component which renders an item.
    /// </summary>
    public class ItemView : IGuiComponent
    {
        /// <summary>
        /// The item to render.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// The transparency of the item.
        /// </summary>
        public float Transparency { get; init; } = 1f;

        /// <summary>
        /// The layer depth to draw at.
        /// </summary>
        public float LayerDepth { get; init; } = 1f;

        /// <summary>
        /// How to draw the stack number, if any.
        /// </summary>
        public StackDrawType DrawStackNumber { get; init; } = StackDrawType.Draw;

        /// <summary>
        /// The color to tint the item.
        /// </summary>
        public Color Color { get; init; } = Color.White;

        /// <summary>
        /// Whether to draw the item's shadow.
        /// </summary>
        public bool DrawShadow { get; init; } = true;

        /// <summary>
        /// Creates a new <see cref="ItemView"/>.
        /// </summary>
        /// <param name="item">The item to show in this view.</param>
        public ItemView(Item item)
        {
            this.Item = item;
        }

        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            return new();
        }

        /// <inheritdoc />
        public void Handle(GuiEvent e, Rectangle bounds)
        {
            e.Draw(
                batch =>
                {
                    var sideLength = Math.Min(bounds.Width, bounds.Height);
                    var scaleSize = sideLength / 64f;
                    this.Item.DrawInMenuCorrected(
                        batch,
                        new(bounds.X, bounds.Y),
                        scaleSize,
                        this.Transparency,
                        this.LayerDepth,
                        this.DrawStackNumber,
                        this.Color,
                        this.DrawShadow,
                        new TopLeftDrawOrigin()
                    );
                }
            );
        }
    }
}
