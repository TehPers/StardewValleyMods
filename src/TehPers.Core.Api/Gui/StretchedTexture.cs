using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Stretches a texture to fill a space.
    /// </summary>
    /// <param name="Texture">The texture to draw.</param>
    public record StretchedTexture(Texture2D Texture) : BaseGuiComponent
    {
        /// <summary>
        /// The source rectangle on the texture.
        /// </summary>
        public Rectangle? SourceRectangle { get; init; } = null;

        /// <summary>
        /// The color to tint the texture.
        /// </summary>
        public Color Color { get; init; } = Color.White;

        /// <summary>
        /// The sprite effects to apply to the texture.
        /// </summary>
        public SpriteEffects Effects { get; init; } = SpriteEffects.None;

        /// <summary>
        /// The layer depth to draw the background on.
        /// </summary>
        public float LayerDepth { get; init; } = 0;

        /// <summary>
        /// The minimum scaled size of this texture. A min scaled width of 2 means that this
        /// texture must be stretched by at least double its original width, for example.
        /// </summary>
        public GuiSize MinScale { get; init; } = GuiSize.Zero;

        /// <summary>
        /// The maximum scaled size of this texture. A max scaled width of 2 means that this
        /// texture can only be stretched up to double its original width, for example.
        /// </summary>
        public PartialGuiSize MaxScale { get; init; } = PartialGuiSize.Empty;

        /// <inheritdoc />
        public override GuiConstraints Constraints => StretchedTexture.GetConstraints(
            this.Texture,
            this.SourceRectangle,
            this.MinScale,
            this.MaxScale
        );

        private static GuiConstraints GetConstraints(
            Texture2D texture,
            Rectangle? sourceRectangle,
            GuiSize minScale,
            PartialGuiSize maxScale
        )
        {
            var sourceWidth = sourceRectangle?.Width ?? texture.Width;
            var sourceHeight = sourceRectangle?.Height ?? texture.Height;
            return new()
            {
                MinSize = new(sourceWidth * minScale.Width, sourceHeight * minScale.Height),
                MaxSize = new(
                    maxScale.Width switch
                    {
                        null => null,
                        { } scale => sourceWidth * scale
                    },
                    maxScale.Height switch
                    {
                        null => null,
                        { } scale => sourceHeight * scale
                    }
                )
            };
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch batch, Rectangle bounds)
        {
            // Don't draw if total area is 0
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            var width = this.MaxScale.Width switch
            {
                null => bounds.Width,
                { } maxScale => Math.Min(
                    bounds.Width,
                    (int)Math.Ceiling(this.Texture.Width * maxScale)
                ),
            };
            var height = this.MaxScale.Height switch
            {
                null => bounds.Height,
                { } maxScale => Math.Min(
                    bounds.Height,
                    (int)Math.Ceiling(this.Texture.Height * maxScale)
                ),
            };

            // Draw the stretched sprite
            batch.Draw(
                this.Texture,
                new(bounds.X, bounds.Y, width, height),
                this.SourceRectangle,
                this.Color,
                0,
                Vector2.Zero,
                this.Effects,
                this.LayerDepth
            );
        }

        /// <inheritdoc />
        public override bool Update(
            GuiEvent e,
            IImmutableDictionary<IGuiComponent, Rectangle> componentBounds,
            [NotNullWhen(true)] out IGuiComponent? newComponent
        )
        {
            newComponent = default;
            return false;
        }
    }
}
