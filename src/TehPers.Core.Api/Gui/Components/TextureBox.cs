using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.Core.Api.Gui.Layouts;

namespace TehPers.Core.Api.Gui.Components
{
    internal record TextureBox(
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
    ) : ComponentWrapper
    {
        public override IGuiComponent Inner => this.CreateInner();

        public GuiSize MinScale { get; init; } = GuiSize.One;

        /// <summary>
        /// The layer depth to draw the component on.
        /// </summary>
        public float? LayerDepth { get; init; }

        private void MaybeAddCell(ILayoutBuilder builder, Rectangle? sourceRectangle, PartialGuiSize maxScale, bool orEmpty)
        {
            if (sourceRectangle is { } rect)
            {
                GuiComponent.Texture(
                    this.Texture,
                    sourceRectangle: rect,
                    minScale: this.MinScale,
                    maxScale: maxScale,
                    layerDepth: this.LayerDepth
                ).AddTo(builder);
            }
            else if (orEmpty)
            {
                GuiComponent.Empty().AddTo(builder);
            }
        }

        private IGuiComponent CreateInner()
        {
            return VerticalLayout.Build(
                    builder =>
                    {
                        // Top row
                        builder.Add(
                            HorizontalLayout.Build(
                                builder =>
                                {
                                    this.MaybeAddCell(builder, this.TopLeft, new(this.MinScale.Width, this.MinScale.Height), false);
                                    this.MaybeAddCell(builder, this.TopCenter, new(null, this.MinScale.Height), true);
                                    this.MaybeAddCell(builder, this.TopRight, new(this.MinScale.Width, this.MinScale.Height), false);
                                }
                            )
                        );

                        // Middle row
                        builder.Add(
                            HorizontalLayout.Build(
                                builder =>
                                {
                                    this.MaybeAddCell(builder, this.CenterLeft, new(this.MinScale.Width, null), false);
                                    this.MaybeAddCell(builder, this.Center, PartialGuiSize.Empty, true);
                                    this.MaybeAddCell(builder, this.CenterRight, new(this.MinScale.Width, null), false);
                                }
                            )
                        );

                        // Bottom row
                        builder.Add(
                            HorizontalLayout.Build(
                                builder =>
                                {
                                    this.MaybeAddCell(builder, this.BottomLeft, new(this.MinScale.Width, this.MinScale.Height), false);
                                    this.MaybeAddCell(builder, this.BottomCenter, new(null, this.MinScale.Height), true);
                                    this.MaybeAddCell(builder, this.BottomRight, new(this.MinScale.Width, this.MinScale.Height), false);
                                }
                            )
                        );
                    }
                );
        }
    }
}
