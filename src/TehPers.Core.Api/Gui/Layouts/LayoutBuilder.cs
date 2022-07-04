using System;

namespace TehPers.Core.Api.Gui.Layouts
{
    /// <summary>
    /// Extension methods for layout builders.
    /// </summary>
    public static class LayoutBuilder
    {
        /// <summary>
        /// Adds this component to a layout.
        /// </summary>
        /// <param name="component">The component to add to the layout.</param>
        /// <param name="builder">The layout builder.</param>
        public static void AddTo(this IGuiComponent component, ILayoutBuilder builder)
        {
            builder.Add(component);
        }

        /// <summary>
        /// Adds a nested vertical layout to this layout.
        /// </summary>
        /// <param name="builder">The outer layout builder.</param>
        /// <param name="addComponents">The components to add to the inner layout.</param>
        public static void Vertical(
            this ILayoutBuilder builder,
            Action<ILayoutBuilder> addComponents
        )
        {
            builder.Add(VerticalLayout.BuildAligned(addComponents));
        }

        /// <summary>
        /// Adds a nested vertical layout to this layout.
        /// </summary>
        /// <param name="builder">The outer layout builder.</param>
        /// <param name="defaultAlignment">The default row alignment.</param>
        /// <param name="addComponents">The components to add to the inner layout.</param>
        public static void Vertical(
            this ILayoutBuilder builder,
            HorizontalAlignment defaultAlignment,
            Action<ILayoutBuilder> addComponents
        )
        {
            builder.Add(VerticalLayout.BuildAligned(addComponents, defaultAlignment));
        }

        /// <summary>
        /// Adds a nested horizontal layout to this layout.
        /// </summary>
        /// <param name="builder">The outer layout builder.</param>
        /// <param name="addComponents">The components to add to the inner layout.</param>
        public static void Horizontal(
            this ILayoutBuilder builder,
            Action<ILayoutBuilder> addComponents
        )
        {
            builder.Add(HorizontalLayout.BuildAligned(addComponents));
        }

        /// <summary>
        /// Adds a nested horizontal layout to this layout.
        /// </summary>
        /// <param name="builder">The outer layout builder.</param>
        /// <param name="defaultAlignment">The default column alignment.</param>
        /// <param name="addComponents">The components to add to the inner layout.</param>
        public static void Horizontal(
            this ILayoutBuilder builder,
            VerticalAlignment defaultAlignment,
            Action<ILayoutBuilder> addComponents
        )
        {
            builder.Add(HorizontalLayout.BuildAligned(addComponents, defaultAlignment));
        }

        /// <summary>
        /// Projects each component added to this layout to a new component.
        /// </summary>
        /// <param name="builder">The layout builder.</param>
        /// <param name="f">A function which projects a component to a new component.</param>
        /// <returns>The new layout builder.</returns>
        public static ILayoutBuilder Select(
            this ILayoutBuilder builder,
            Func<IGuiComponent, IGuiComponent> f
        )
        {
            return new MappedLayoutBuilder(builder, f);
        }

        private class MappedLayoutBuilder : ILayoutBuilder
        {
            private readonly ILayoutBuilder inner;
            private readonly Func<IGuiComponent, IGuiComponent> f;

            public MappedLayoutBuilder(ILayoutBuilder inner, Func<IGuiComponent, IGuiComponent> f)
            {
                this.inner = inner;
                this.f = f;
            }

            public void Add(IGuiComponent component)
            {
                this.inner.Add(this.f(component));
            }

            public IGuiComponent Build()
            {
                return this.inner.Build();
            }
        }
    }
}
