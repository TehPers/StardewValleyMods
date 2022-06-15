using StardewValley.Menus;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Extension methods for components.
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// Converts this component to an <see cref="IClickableMenu"/>.
        /// </summary>
        /// <typeparam name="TState">The type of the component's state.</typeparam>
        /// <param name="component">The component to turn into a menu.</param>
        /// <returns>The menu.</returns>
        public static IClickableMenu ToMenu<TState>(this IGuiComponent<TState> component)
        {
            return new ManagedMenu<TState>(component);
        }

        /// <summary>
        /// Adds a background to this component.
        /// </summary>
        /// <typeparam name="TFgState">The type of the foreground component's state.</typeparam>
        /// <typeparam name="TBgState">The type of the background component's state.</typeparam>
        /// <param name="component">The component to add a background to.</param>
        /// <param name="background">The background component.</param>
        /// <returns>A component that applies a background to the inner component.</returns>
        public static WithBackground<TFgState, TBgState> WithBackground<TFgState, TBgState>(
            this IGuiComponent<TFgState> component,
            IGuiComponent<TBgState> background
        )
        {
            return new(component, background);
        }

        /// <summary>
        /// Horizontally aligns this component in its parent.
        /// </summary>
        /// <typeparam name="TState">The type of the inner component's state.</typeparam>
        /// <param name="component">The component to horizontally align.</param>
        /// <param name="horizontal">The horizontal alignment to apply.</param>
        /// <returns>The aligned component.</returns>
        public static HorizontalAlign<TState> Aligned<TState>(
            this IGuiComponent<TState> component,
            HorizontalAlignment horizontal
        )
        {
            return new(component, horizontal);
        }

        /// <summary>
        /// Vertically aligns this component in its parent.
        /// </summary>
        /// <typeparam name="TState">The type of the inner component's state.</typeparam>
        /// <param name="component">The component to vertically align.</param>
        /// <param name="vertical">The vertical alignment to apply.</param>
        /// <returns>The aligned component.</returns>
        public static VerticalAlign<TState> Aligned<TState>(
            this IGuiComponent<TState> component,
            VerticalAlignment vertical
        )
        {
            return new(component, vertical);
        }

        /// <summary>
        /// Fully aligns this component in its parent.
        /// </summary>
        /// <typeparam name="TState">The type of the inner component's state.</typeparam>
        /// <param name="component">The component to align.</param>
        /// <param name="horizontal">The horizontal alignment to apply.</param>
        /// <param name="vertical">The vertical alignment to apply.</param>
        /// <returns>The aligned component.</returns>
        public static VerticalAlign<TState> Aligned<TState>(
            this IGuiComponent<TState> component,
            HorizontalAlignment horizontal,
            VerticalAlignment vertical
        )
        {
            return component.Aligned(horizontal).Aligned(vertical);
        }

        /// <summary>
        /// Adds padding to this component.
        /// </summary>
        /// <typeparam name="TState">The type of the inner component's state.</typeparam>
        /// <param name="component">The component to add padding to.</param>
        /// <param name="sides">The amount of padding on all sides.</param>
        /// <returns>The padded component.</returns>
        public static WithPadding<TState> WithPadding<TState>(
            this IGuiComponent<TState> component,
            float sides
        )
        {
            return component.WithPadding(sides, sides, sides, sides);
        }

        /// <summary>
        /// Adds padding to this component.
        /// </summary>
        /// <typeparam name="TState">The type of the inner component's state.</typeparam>
        /// <param name="component">The component to add padding to.</param>
        /// <param name="leftRight">The amount of padding on the left and right.</param>
        /// <param name="topBottom">The amount of padding on the top and bottom.</param>
        /// <returns>The padded component.</returns>
        public static WithPadding<TState> WithPadding<TState>(
            this IGuiComponent<TState> component,
            float leftRight,
            float topBottom
        )
        {
            return component.WithPadding(leftRight, leftRight, topBottom, topBottom);
        }

        /// <summary>
        /// Adds padding to this component.
        /// </summary>
        /// <typeparam name="TState">The type of the inner component's state.</typeparam>
        /// <param name="component">The component to add padding to.</param>
        /// <param name="left">The amount of padding on the left.</param>
        /// <param name="right">The amount of padding on the right.</param>
        /// <param name="top">The amount of padding on the top.</param>
        /// <param name="bottom">The amount of padding on the bottom.</param>
        /// <returns>The padded component.</returns>
        public static WithPadding<TState> WithPadding<TState>(
            this IGuiComponent<TState> component,
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
        /// <typeparam name="TState">The type of the inner component's state.</typeparam>
        /// <param name="component">The component to shrink.</param>
        /// <returns>The shrunk component.</returns>
        public static Shrink<TState> Shrink<TState>(this IGuiComponent<TState> component)
        {
            return new(component);
        }

        /// <summary>
        /// Wraps a component in an intermediate, non-generic type.
        /// </summary>
        /// <typeparam name="TState">The type of the inner component's state.</typeparam>
        /// <param name="component">The inner component.</param>
        /// <returns>The wrapped component.</returns>
        public static WrappedComponent Wrapped<TState>(this IGuiComponent<TState> component)
        {
            return WrappedComponent.Of(component);
        }
    }
}
