using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Extensions;
using TehPers.Core.Gui.Api.Guis;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="IStretchedTexture"/>
internal record StretchedTexture
    (IGuiBuilder Builder, Texture2D Texture) : BaseGuiComponent(Builder), IStretchedTexture
{
    public Rectangle? SourceRectangle { get; init; }
    public Color Color { get; init; } = Color.White;
    public SpriteEffects Effects { get; init; } = SpriteEffects.None;
    public float LayerDepth { get; init; }
    public IGuiSize MinScale { get; init; } = GuiSize.One;
    public IPartialGuiSize MaxScale { get; init; } = PartialGuiSize.One;

    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        var sourceWidth = this.SourceRectangle?.Width ?? this.Texture.Width;
        var sourceHeight = this.SourceRectangle?.Height ?? this.Texture.Height;
        return new GuiConstraints(
            new GuiSize(sourceWidth * this.MinScale.Width, sourceHeight * this.MinScale.Height),
            new PartialGuiSize(
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
        );
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        e.Draw(
            batch =>
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
        );
    }

    /// <inheritdoc />
    public IStretchedTexture WithLayerDepth(float layerDepth)
    {
        return this with {LayerDepth = layerDepth};
    }

    /// <inheritdoc />
    public IStretchedTexture WithTexture(Texture2D texture)
    {
        return this with {Texture = texture};
    }

    /// <inheritdoc />
    public IStretchedTexture WithSourceRectangle(Rectangle? sourceRectangle)
    {
        return this with {SourceRectangle = sourceRectangle};
    }

    /// <inheritdoc />
    public IStretchedTexture WithColor(Color color)
    {
        return this with {Color = color};
    }

    /// <inheritdoc />
    public IStretchedTexture WithSpriteEffects(SpriteEffects effects)
    {
        return this with {Effects = effects};
    }

    /// <inheritdoc />
    public IStretchedTexture WithMinScale(IGuiSize minScale)
    {
        return this with {MinScale = minScale};
    }

    /// <inheritdoc />
    public IStretchedTexture WithMaxScale(IPartialGuiSize maxScale)
    {
        return this with {MaxScale = maxScale};
    }
}
