using System;
using System.Collections.Generic;

namespace TehPers.Core.Api.Gui.Layouts
{
    /// <summary>
    /// Extension methods for layout builders.
    /// </summary>
    public static class LayoutBuilder
    {
        /// <summary>
        /// Adds a new component to this layout.
        /// </summary>
        /// <param name="layout">The layout builder.</param>
        /// <param name="component">The component to add.</param>
        /// <typeparam name="TResponse">The type of the component's response.</typeparam>
        /// <typeparam name="TLayout">The type of the resulting layout.</typeparam>
        public static void Add<TResponse, TLayout>(
            this ILayoutBuilder<Unit, TLayout> layout,
            IGuiComponent<TResponse> component
        )
        {
            layout.Add(component.IgnoreResponse());
        }

        /// <summary>
        /// Adds a nested vertical layout to this layout.
        /// </summary>
        /// <typeparam name="TLayout">The type of the outer layout.</typeparam>
        /// <param name="layout">The outer layout.</param>
        /// <param name="addComponents">The components to add to the inner layout.</param>
        public static void Vertical<TLayout>(
            this ILayoutBuilder<Unit, TLayout> layout,
            Action<ILayoutBuilder<Unit, VerticalLayout<Unit>>> addComponents
        )
        {
            layout.Add(VerticalLayout.Build(addComponents));
        }

        /// <summary>
        /// Adds a nested vertical layout to this layout.
        /// </summary>
        /// <typeparam name="TResponse">The type of the inner layout's components' responses.</typeparam>
        /// <typeparam name="TLayout">The type of the outer layout.</typeparam>
        /// <param name="layout">The outer layout.</param>
        /// <param name="addComponents">The components to add to the inner layout.</param>
        public static void Vertical<TResponse, TLayout>(
            this ILayoutBuilder<IEnumerable<VerticalLayout<TResponse>.ResponseItem>, TLayout>
                layout,
            Action<ILayoutBuilder<TResponse, VerticalLayout<TResponse>>> addComponents
        )
        {
            layout.Add(VerticalLayout.Build(addComponents));
        }

        /// <summary>
        /// Adds a nested horizontal layout to this layout.
        /// </summary>
        /// <typeparam name="TLayout">The type of the outer layout.</typeparam>
        /// <param name="layout">The outer layout.</param>
        /// <param name="addComponents">The components to add to the inner layout.</param>
        public static void Horizontal<TLayout>(
            this ILayoutBuilder<Unit, TLayout> layout,
            Action<ILayoutBuilder<Unit, HorizontalLayout<Unit>>> addComponents
        )
        {
            layout.Add(HorizontalLayout.Build(addComponents));
        }

        /// <summary>
        /// Adds a nested horizontal layout to this layout.
        /// </summary>
        /// <typeparam name="TResponse">The type of the inner layout's components' responses.</typeparam>
        /// <typeparam name="TLayout">The type of the outer layout.</typeparam>
        /// <param name="layout">The outer layout.</param>
        /// <param name="addComponents">The components to add to the inner layout.</param>
        public static void Horizontal<TResponse, TLayout>(
            this ILayoutBuilder<IEnumerable<HorizontalLayout<TResponse>.ResponseItem>, TLayout>
                layout,
            Action<ILayoutBuilder<TResponse, HorizontalLayout<TResponse>>> addComponents
        )
        {
            layout.Add(HorizontalLayout.Build(addComponents));
        }
    }
}
