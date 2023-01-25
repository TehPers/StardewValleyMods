using Microsoft.Xna.Framework;
using StardewValley;

namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A component which renders an item.
/// </summary>
public interface IItemView : IGuiComponent, IWithLayerDepth<IItemView>
{
    /// <summary>
    /// Sets the item to show in this view.
    /// </summary>
    /// <param name="item">The item to show.</param>
    /// <returns>The resulting component.</returns>
    IItemView WithItem(Item item);

    /// <summary>
    /// Sets the transparency of the item.
    /// </summary>
    /// <param name="transparency">The transparency of the item.</param>
    /// <returns>The resulting component.</returns>
    IItemView WithTransparency(float transparency);

    /// <summary>
    /// Sets the side length of this item view.
    /// </summary>
    /// <param name="sideLength">The side length.</param>
    /// <returns>The resulting component.</returns>
    IItemView WithSideLength(float sideLength);

    /// <summary>
    /// Sets how to draw the stack number, if any.
    /// </summary>
    /// <param name="drawType">How the stack number should be drawn.</param>
    /// <returns>The resulting component.</returns>
    IItemView WithStackDrawType(StackDrawType drawType);

    /// <summary>
    /// Sets the color to tint the item.
    /// </summary>
    /// <param name="color">The tint color.</param>
    /// <returns>The resulting component.</returns>
    IItemView WithColor(Color color);

    /// <summary>
    /// Sets whether to draw the item's shadow.
    /// </summary>
    /// <param name="drawShadow">Whether to draw the shadow.</param>
    /// <returns>The resulting component.</returns>
    IItemView WithShadow(bool drawShadow);
}
