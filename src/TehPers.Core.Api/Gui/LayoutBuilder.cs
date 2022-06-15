using System;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Extension methods for layout builders.
    /// </summary>
    public static class LayoutBuilder
    {
        /// <summary>
        /// Adds a nested vertical layout.
        /// </summary>
        /// <typeparam name="TLayout">The type of the outer layout.</typeparam>
        /// <param name="builder">The outer layout builder.</param>
        /// <param name="addComponents">A callback for building the inner layout.</param>
        public static void VerticalLayout<TLayout>(
            this ILayoutBuilder<WrappedComponent.State, TLayout> builder,
            Action<VerticalLayout.Builder> addComponents
        )
        {
            var innerBuilder = new VerticalLayout.Builder();
            addComponents(innerBuilder);
            builder.Add(innerBuilder.Build().Wrapped());
        }
    }
}
