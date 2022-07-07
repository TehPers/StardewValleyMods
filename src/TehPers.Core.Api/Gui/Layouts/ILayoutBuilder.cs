using TehPers.Core.Api.Gui.Components;

namespace TehPers.Core.Api.Gui.Layouts
{
    /// <summary>
    /// A builder for a GUI layout.
    /// </summary>
    public interface ILayoutBuilder
    {
        /// <summary>
        /// Adds a new component to this layout.
        /// </summary>
        /// <param name="component">The component to add to the layout.</param>
        void Add(IGuiComponent component);

        /// <summary>
        /// Builds the layout from this builder.
        /// </summary>
        /// <returns>The built layout.</returns>
        IGuiComponent Build();
    }
}
