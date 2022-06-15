using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Stretches a texture to fill a space.
    /// </summary>
    /// <param name="Texture">The texture to draw.</param>
    public record StretchedTexture(Texture2D Texture) : IGuiComponent<StretchedTexture.State>
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
        public GuiConstraints GetConstraints()
        {
            var sourceWidth = this.SourceRectangle?.Width ?? this.Texture.Width;
            var sourceHeight = this.SourceRectangle?.Height ?? this.Texture.Height;
            return new()
            {
                MinSize = new(
                    sourceWidth * this.MinScale.Width,
                    sourceHeight * this.MinScale.Height
                ),
                MaxSize = new(
                    this.MaxScale.Width switch
                    {
                        null => null,
                        { } scale => sourceWidth * scale
                    },
                    this.MaxScale.Height switch
                    {
                        null => null,
                        { } scale => sourceHeight * scale
                    }
                )
            };
        }

        /// <inheritdoc />
        public State Initialize(Rectangle bounds)
        {
            return new(bounds);
        }

        /// <inheritdoc />
        public State Reposition(State state, Rectangle bounds)
        {
            return new(bounds);
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch batch, State state)
        {
            // Don't draw if total area is 0
            if (state.Bounds.Width <= 0 || state.Bounds.Height <= 0)
            {
                return;
            }

            var width = this.MaxScale.Width switch
            {
                null => state.Bounds.Width,
                { } maxScale => Math.Min(
                    state.Bounds.Width,
                    (int)Math.Ceiling(this.Texture.Width * maxScale)
                ),
            };
            var height = this.MaxScale.Height switch
            {
                null => state.Bounds.Height,
                { } maxScale => Math.Min(
                    state.Bounds.Height,
                    (int)Math.Ceiling(this.Texture.Height * maxScale)
                ),
            };

            // Draw the stretched sprite
            batch.Draw(
                this.Texture,
                new(state.Bounds.X, state.Bounds.Y, width, height),
                this.SourceRectangle,
                this.Color,
                0,
                Vector2.Zero,
                this.Effects,
                this.LayerDepth
            );
        }

        /// <inheritdoc />
        public bool Update(GuiEvent e, State state, [NotNullWhen(true)] out State? nextState)
        {
            nextState = default;
            return false;
        }

        /// <summary>
        /// The state of a <see cref="StretchedTexture"/> component.
        /// </summary>
        /// <param name="Bounds"></param>
        public record State(Rectangle Bounds);
    }
}
