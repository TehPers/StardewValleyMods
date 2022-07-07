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
    }
}
