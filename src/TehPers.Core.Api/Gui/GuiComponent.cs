using StardewValley.Menus;
using System;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Extension methods for components.
    /// </summary>
    public static class GuiComponent
    {
        /// <summary>
        /// Converts this component to an <see cref="IClickableMenu"/>.
        /// </summary>
        /// <param name="component">The component to turn into a menu.</param>
        /// <returns>The menu.</returns>
        public static IClickableMenu ToMenu(this IGuiComponent component)
        {
            return new SimpleManagedMenu(component);
        }

        /// <summary>
        /// Adds a background to this component.
        /// </summary>
        /// <param name="component">The component to add a background to.</param>
        /// <param name="background">The background component.</param>
        /// <returns>A component that applies a background to the inner component.</returns>
        public static WithBackground WithBackground(
            this IGuiComponent component,
            IGuiComponent background
        )
        {
            return new(component, background);
        }

        /// <summary>
        /// Horizontally aligns this component in its parent.
        /// </summary>
        /// <param name="component">The component to horizontally align.</param>
        /// <param name="horizontal">The horizontal alignment to apply.</param>
        /// <returns>The aligned component.</returns>
        public static HorizontalAlign Aligned(
            this IGuiComponent component,
            HorizontalAlignment horizontal
        )
        {
            return new(component, horizontal);
        }

        /// <summary>
        /// Vertically aligns this component in its parent.
        /// </summary>
        /// <param name="component">The component to vertically align.</param>
        /// <param name="vertical">The vertical alignment to apply.</param>
        /// <returns>The aligned component.</returns>
        public static VerticalAlign Aligned(
            this IGuiComponent component,
            VerticalAlignment vertical
        )
        {
            return new(component, vertical);
        }

        /// <summary>
        /// Fully aligns this component in its parent.
        /// </summary>
        /// <param name="component">The component to align.</param>
        /// <param name="horizontal">The horizontal alignment to apply.</param>
        /// <param name="vertical">The vertical alignment to apply.</param>
        /// <returns>The aligned component.</returns>
        public static VerticalAlign Aligned(
            this IGuiComponent component,
            HorizontalAlignment horizontal,
            VerticalAlignment vertical
        )
        {
            return component.Aligned(horizontal).Aligned(vertical);
        }

        /// <summary>
        /// Adds padding to this component.
        /// </summary>
        /// <param name="component">The component to add padding to.</param>
        /// <param name="sides">The amount of padding on all sides.</param>
        /// <returns>The padded component.</returns>
        public static WithPadding WithPadding(this IGuiComponent component, float sides)
        {
            return component.WithPadding(sides, sides, sides, sides);
        }

        /// <summary>
        /// Adds padding to this component.
        /// </summary>
        /// <param name="component">The component to add padding to.</param>
        /// <param name="leftRight">The amount of padding on the left and right.</param>
        /// <param name="topBottom">The amount of padding on the top and bottom.</param>
        /// <returns>The padded component.</returns>
        public static WithPadding WithPadding(
            this IGuiComponent component,
            float leftRight,
            float topBottom
        )
        {
            return component.WithPadding(leftRight, leftRight, topBottom, topBottom);
        }

        /// <summary>
        /// Adds padding to this component.
        /// </summary>
        /// <param name="component">The component to add padding to.</param>
        /// <param name="left">The amount of padding on the left.</param>
        /// <param name="right">The amount of padding on the right.</param>
        /// <param name="top">The amount of padding on the top.</param>
        /// <param name="bottom">The amount of padding on the bottom.</param>
        /// <returns>The padded component.</returns>
        public static WithPadding WithPadding(
            this IGuiComponent component,
            float left,
            float right,
            float top,
            float bottom
        )
        {
            return new(component, left, right, top, bottom);
        }

        /// <summary>
        /// Shrinks this component to its minimum size.
        /// </summary>
        /// <param name="component">The component to shrink.</param>
        /// <returns>The shrunk component.</returns>
        public static Shrink Shrink(this IGuiComponent component)
        {
            return new(component);
        }

        /// <summary>
        /// Executes an action when this control is clicked.
        /// </summary>
        /// <param name="component">The component to check for clicks for.</param>
        /// <param name="onClick">The action to perform.</param>
        /// <returns>The component wrapped in a button.</returns>
        public static Button OnClick(this IGuiComponent component, Action<ClickType> onClick)
        {
            return new(component, onClick);
        }
    }
}
