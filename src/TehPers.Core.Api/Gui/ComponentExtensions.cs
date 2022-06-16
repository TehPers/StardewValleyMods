using StardewValley.Menus;
using System;
using TehPers.Core.Api.Gui.Combinators;

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
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        /// <param name="component">The component to turn into a menu.</param>
        /// <returns>The menu.</returns>
        public static IClickableMenu ToMenu<TResponse>(this IGuiComponent<TResponse> component)
        {
            return component.IgnoreResponse().ToMenu();
        }

        /// <summary>
        /// Converts this component to an <see cref="IClickableMenu"/>.
        /// </summary>
        /// <param name="component">The component to turn into a menu.</param>
        /// <returns>The menu.</returns>
        public static IClickableMenu ToMenu(this IGuiComponent<Unit> component)
        {
            return new SimpleManagedMenu(component);
        }

        /// <summary>
        /// Maps the response from a component to another response.
        /// </summary>
        /// <typeparam name="T1">The original response type.</typeparam>
        /// <typeparam name="T2">The new response type.</typeparam>
        /// <param name="component">The inner component.</param>
        /// <param name="map">A function which maps the original response to a new response.</param>
        /// <returns>The mapped component.</returns>
        public static IGuiComponent<T2> Select<T1, T2>(
            this IGuiComponent<T1> component,
            Func<T1, T2> map
        )
        {
            return new MappedComponent<T1, T2>(component, map);
        }

        /// <summary>
        /// Ignores the response of a component.
        /// </summary>
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        /// <param name="component">The component to ignore the response of.</param>
        /// <returns>The ignored component.</returns>
        public static IGuiComponent IgnoreResponse<TResponse>(
            this IGuiComponent<TResponse> component
        )
        {
            return new IgnoredComponent<TResponse>(component);
        }

        /// <summary>
        /// Adds a background to this component.
        /// </summary>
        /// <typeparam name="TFgResponse">The type of the foreground component's response.</typeparam>
        /// <typeparam name="TBgResponse">The type of the background component's response.</typeparam>
        /// <param name="component">The component to add a background to.</param>
        /// <param name="background">The background component.</param>
        /// <returns>A component that applies a background to the inner component.</returns>
        public static WithBackground<TFgResponse, TBgResponse>
            WithBackground<TFgResponse, TBgResponse>(
                this IGuiComponent<TFgResponse> component,
                IGuiComponent<TBgResponse> background
            )
        {
            return new(component, background);
        }

        /// <summary>
        /// Horizontally aligns this component in its parent.
        /// </summary>
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        /// <param name="component">The component to horizontally align.</param>
        /// <param name="horizontal">The horizontal alignment to apply.</param>
        /// <returns>The aligned component.</returns>
        public static HorizontalAlign<TResponse> Aligned<TResponse>(
            this IGuiComponent<TResponse> component,
            HorizontalAlignment horizontal
        )
        {
            return new(component, horizontal);
        }

        /// <summary>
        /// Vertically aligns this component in its parent.
        /// </summary>
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        /// <param name="component">The component to vertically align.</param>
        /// <param name="vertical">The vertical alignment to apply.</param>
        /// <returns>The aligned component.</returns>
        public static VerticalAlign<TResponse> Aligned<TResponse>(
            this IGuiComponent<TResponse> component,
            VerticalAlignment vertical
        )
        {
            return new(component, vertical);
        }

        /// <summary>
        /// Fully aligns this component in its parent.
        /// </summary>
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        /// <param name="component">The component to align.</param>
        /// <param name="horizontal">The horizontal alignment to apply.</param>
        /// <param name="vertical">The vertical alignment to apply.</param>
        /// <returns>The aligned component.</returns>
        public static VerticalAlign<TResponse> Aligned<TResponse>(
            this IGuiComponent<TResponse> component,
            HorizontalAlignment horizontal,
            VerticalAlignment vertical
        )
        {
            return component.Aligned(horizontal).Aligned(vertical);
        }

        /// <summary>
        /// Adds padding to this component.
        /// </summary>
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        /// <param name="component">The component to add padding to.</param>
        /// <param name="sides">The amount of padding on all sides.</param>
        /// <returns>The padded component.</returns>
        public static WithPadding<TResponse> WithPadding<TResponse>(
            this IGuiComponent<TResponse> component,
            float sides
        )
        {
            return component.WithPadding(sides, sides, sides, sides);
        }

        /// <summary>
        /// Adds padding to this component.
        /// </summary>
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        /// <param name="component">The component to add padding to.</param>
        /// <param name="leftRight">The amount of padding on the left and right.</param>
        /// <param name="topBottom">The amount of padding on the top and bottom.</param>
        /// <returns>The padded component.</returns>
        public static WithPadding<TResponse> WithPadding<TResponse>(
            this IGuiComponent<TResponse> component,
            float leftRight,
            float topBottom
        )
        {
            return component.WithPadding(leftRight, leftRight, topBottom, topBottom);
        }

        /// <summary>
        /// Adds padding to this component.
        /// </summary>
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        /// <param name="component">The component to add padding to.</param>
        /// <param name="left">The amount of padding on the left.</param>
        /// <param name="right">The amount of padding on the right.</param>
        /// <param name="top">The amount of padding on the top.</param>
        /// <param name="bottom">The amount of padding on the bottom.</param>
        /// <returns>The padded component.</returns>
        public static WithPadding<TResponse> WithPadding<TResponse>(
            this IGuiComponent<TResponse> component,
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
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        /// <param name="component">The component to shrink.</param>
        /// <returns>The shrunk component.</returns>
        public static Shrink<TResponse> Shrink<TResponse>(this IGuiComponent<TResponse> component)
        {
            return new(component);
        }
    }
}
