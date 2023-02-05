using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Components.Layouts;
using TehPers.Core.Gui.Api.Extensions;
using TehPers.Core.Gui.Api.Guis;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="ITextureBox"/>
internal record TextureBox(
    IGuiBuilder Builder,
    Texture2D Texture,
    Rectangle? TopLeft,
    Rectangle? TopCenter,
    Rectangle? TopRight,
    Rectangle? CenterLeft,
    Rectangle? Center,
    Rectangle? CenterRight,
    Rectangle? BottomLeft,
    Rectangle? BottomCenter,
    Rectangle? BottomRight
) : BaseGuiComponent(Builder), ITextureBox
{
    public IGuiSize MinScale { get; init; } = GuiSize.One;
    public float LayerDepth { get; init; }

    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        return this.CreateInner().GetConstraints();
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        this.CreateInner().Handle(e, bounds);
    }

    private void MaybeAddCell(
        ILayoutBuilder builder,
        Rectangle? sourceRectangle,
        PartialGuiSize maxScale,
        bool orEmpty
    )
    {
        if (sourceRectangle is { } rect)
        {
            this.GuiBuilder.Texture(this.Texture)
                .WithSourceRectangle(rect)
                .WithMinScale(this.MinScale)
                .WithMaxScale(maxScale)
                .WithLayerDepth(this.LayerDepth)
                .AddTo(builder);
        }
        else if (orEmpty)
        {
            this.GuiBuilder.Empty().AddTo(builder);
        }
    }

    private IGuiComponent CreateInner()
    {
        return this.GuiBuilder.VerticalLayout(
            builder =>
            {
                // Top row
                this.GuiBuilder.HorizontalLayout(
                        builder =>
                        {
                            this.MaybeAddCell(
                                builder,
                                this.TopLeft,
                                new(this.MinScale.Width, this.MinScale.Height),
                                false
                            );
                            this.MaybeAddCell(
                                builder,
                                this.TopCenter,
                                new(null, this.MinScale.Height),
                                true
                            );
                            this.MaybeAddCell(
                                builder,
                                this.TopRight,
                                new(this.MinScale.Width, this.MinScale.Height),
                                false
                            );
                        }
                    )
                    .AddTo(builder);

                // Middle row
                this.GuiBuilder.HorizontalLayout(
                        builder =>
                        {
                            this.MaybeAddCell(
                                builder,
                                this.CenterLeft,
                                new(this.MinScale.Width, null),
                                false
                            );
                            this.MaybeAddCell(builder, this.Center, PartialGuiSize.Empty, true);
                            this.MaybeAddCell(
                                builder,
                                this.CenterRight,
                                new(this.MinScale.Width, null),
                                false
                            );
                        }
                    )
                    .AddTo(builder);

                // Bottom row
                this.GuiBuilder.HorizontalLayout(
                        builder =>
                        {
                            this.MaybeAddCell(
                                builder,
                                this.BottomLeft,
                                new(this.MinScale.Width, this.MinScale.Height),
                                false
                            );
                            this.MaybeAddCell(
                                builder,
                                this.BottomCenter,
                                new(null, this.MinScale.Height),
                                true
                            );
                            this.MaybeAddCell(
                                builder,
                                this.BottomRight,
                                new(this.MinScale.Width, this.MinScale.Height),
                                false
                            );
                        }
                    )
                    .AddTo(builder);
            }
        );
    }

    /// <inheritdoc />
    public ITextureBox WithLayerDepth(float layerDepth)
    {
        return this with {LayerDepth = layerDepth};
    }

    /// <inheritdoc />
    public ITextureBox WithTexture(Texture2D texture)
    {
        return this with {Texture = texture};
    }

    /// <inheritdoc />
    public ITextureBox WithTopLeft(Rectangle? topLeft)
    {
        return this with {TopLeft = topLeft};
    }

    /// <inheritdoc />
    public ITextureBox WithTopCenter(Rectangle? topCenter)
    {
        return this with {TopCenter = topCenter};
    }

    /// <inheritdoc />
    public ITextureBox WithTopRight(Rectangle? topRight)
    {
        return this with {TopRight = topRight};
    }

    /// <inheritdoc />
    public ITextureBox WithCenterLeft(Rectangle? centerLeft)
    {
        return this with {CenterLeft = centerLeft};
    }

    /// <inheritdoc />
    public ITextureBox WithCenter(Rectangle? center)
    {
        return this with {Center = center};
    }

    /// <inheritdoc />
    public ITextureBox WithCenterRight(Rectangle? centerRight)
    {
        return this with {CenterRight = centerRight};
    }

    /// <inheritdoc />
    public ITextureBox WithBottomLeft(Rectangle? bottomLeft)
    {
        return this with {BottomLeft = bottomLeft};
    }

    /// <inheritdoc />
    public ITextureBox WithBottomCenter(Rectangle? bottomCenter)
    {
        return this with {BottomCenter = bottomCenter};
    }

    /// <inheritdoc />
    public ITextureBox WithBottomRight(Rectangle? bottomRight)
    {
        return this with {BottomRight = bottomRight};
    }

    /// <inheritdoc />
    ITextureBox ITextureBox.WithMinScale(IGuiSize minScale)
    {
        return this with {MinScale = minScale};
    }
}
