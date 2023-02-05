using Microsoft.Xna.Framework;
using StardewValley;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Extensions;
using TehPers.Core.Gui.Api.Guis;
using TehPers.Core.Gui.Extensions;
using TehPers.Core.Gui.Extensions.Drawing;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="IItemView"/>
internal record ItemView(IGuiBuilder Builder, Item Item) : BaseGuiComponent(Builder), IItemView
{
    public float Transparency { get; init; } = 1f;
    public float LayerDepth { get; init; } = 1f;
    public float SideLength { get; init; } = 64f;
    public StackDrawType StackDrawType { get; init; } = StackDrawType.Draw;
    public Color Color { get; init; } = Color.White;
    public bool DrawShadow { get; init; } = true;

    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        return new GuiConstraints(
            new GuiSize(this.SideLength, this.SideLength),
            new PartialGuiSize(this.SideLength, this.SideLength)
        );
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        e.Draw(
            batch =>
            {
                var scaleSize = this.SideLength / 64f;
                this.Item.DrawInMenuCorrected(
                    batch,
                    new(bounds.X, bounds.Y),
                    scaleSize,
                    this.Transparency,
                    this.LayerDepth,
                    this.StackDrawType,
                    this.Color,
                    this.DrawShadow,
                    new TopLeftDrawOrigin()
                );
            }
        );
    }

    /// <inheritdoc />
    public IItemView WithLayerDepth(float layerDepth)
    {
        return this with {LayerDepth = layerDepth};
    }

    /// <inheritdoc />
    public IItemView WithItem(Item item)
    {
        return this with {Item = item};
    }

    /// <inheritdoc />
    public IItemView WithTransparency(float transparency)
    {
        return this with {Transparency = transparency};
    }

    /// <inheritdoc />
    public IItemView WithSideLength(float sideLength)
    {
        return this with {SideLength = sideLength};
    }

    /// <inheritdoc />
    public IItemView WithStackDrawType(StackDrawType drawType)
    {
        return this with {StackDrawType = drawType};
    }

    /// <inheritdoc />
    public IItemView WithColor(Color color)
    {
        return this with {Color = color};
    }

    /// <inheritdoc />
    public IItemView WithShadow(bool drawShadow)
    {
        return this with {DrawShadow = drawShadow};
    }
}
