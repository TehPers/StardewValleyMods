namespace TehPers.Core.Api.Gui.Layouts
{
    /// <summary>
    /// A builder for a GUI layout.
    /// </summary>
    /// <typeparam name="TResponse">The type of response from the components.</typeparam>
    /// <typeparam name="TLayout">The layout type.</typeparam>
    // TODO: remove TLayout?
    public interface ILayoutBuilder<in TResponse, out TLayout>
    {
        /// <summary>
        /// Adds a new component to this layout.
        /// </summary>
        /// <param name="component">The component to add to the layout.</param>
        void Add(IGuiComponent<TResponse> component);

        /// <summary>
        /// Builds the layout from this builder.
        /// </summary>
        /// <returns>The built layout.</returns>
        TLayout Build();
    }
}
