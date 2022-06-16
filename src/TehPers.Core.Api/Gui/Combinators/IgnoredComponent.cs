using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Gui.Combinators
{
    /// <summary>
    /// Ignores the response of a component.
    /// </summary>
    /// <typeparam name="TResponse">The inner component's response type.</typeparam>
    /// <param name="Inner">The inner component.</param>
    public record IgnoredComponent<TResponse>(IGuiComponent<TResponse> Inner) : IGuiComponent
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            return this.Inner.GetConstraints();
        }

        /// <inheritdoc />
        public void Handle(GuiEvent e, Rectangle bounds)
        {
            this.Inner.Handle(e, bounds);
        }
    }
}
