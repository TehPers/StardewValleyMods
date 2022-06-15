namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A builder for a GUI layout.
    /// </summary>
    /// <typeparam name="TState">The type of the state of the layout's components.</typeparam>
    /// <typeparam name="TLayout">The layout type.</typeparam>
    public interface ILayoutBuilder<TState, out TLayout>
    {
        /// <summary>
        /// Adds a new component to this layout.
        /// </summary>
        /// <param name="component">The component to add to the layout.</param>
        void Add(IGuiComponent<TState> component);

        /// <summary>
        /// Builds the layout from this builder.
        /// </summary>
        /// <returns>The built layout.</returns>
        TLayout Build();
    }
}
