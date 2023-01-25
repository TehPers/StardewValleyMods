using System;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Components.Layouts;

namespace TehPers.Core.Gui.Api.Extensions;

/// <summary>
/// Extension methods for working with <see cref="ILayoutBuilder"/>.
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
    /// Creates a new layout builder which aligns all the components added to it.
    /// </summary>
    /// <param name="builder">The inner layout builder.</param>
    /// <param name="horizontal">The horizontal alignment, if any.</param>
    /// <param name="vertical">The vertical alignment, if any.</param>
    /// <returns>The new layout builder.</returns>
    public static ILayoutBuilder Aligned(
        this ILayoutBuilder builder,
        HorizontalAlignment? horizontal = null,
        VerticalAlignment? vertical = null
    )
    {
        return builder.Select(c => c.Aligned(horizontal, vertical));
    }

    /// <summary>
    /// Creates a new layout builder which applies a function to each component added to it.
    /// </summary>
    /// <param name="builder">The layout builder.</param>
    /// <param name="mapComponent">A function which maps each component to a new component.</param>
    /// <returns>The new layout builder.</returns>
    public static ILayoutBuilder Select(
        this ILayoutBuilder builder,
        Func<IGuiComponent, IGuiComponent> mapComponent
    )
    {
        return new MappedLayoutBuilder(builder, mapComponent);
    }

    private class MappedLayoutBuilder : ILayoutBuilder
    {
        private readonly ILayoutBuilder inner;
        private readonly Func<IGuiComponent, IGuiComponent> mapComponent;

        public MappedLayoutBuilder(
            ILayoutBuilder layoutBuilder,
            Func<IGuiComponent, IGuiComponent> mapComponent
        )
        {
            this.inner = layoutBuilder ?? throw new ArgumentNullException(nameof(layoutBuilder));
            this.mapComponent =
                mapComponent ?? throw new ArgumentNullException(nameof(mapComponent));
        }

        public void Add(IGuiComponent component)
        {
            this.inner.Add(this.mapComponent(component));
        }
    }
}
