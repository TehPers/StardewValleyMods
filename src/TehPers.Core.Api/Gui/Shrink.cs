using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Shrinks a GUI component to its minimum size.
    /// </summary>
    /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
    /// <param name="Inner">The inner component.</param>
    public record Shrink<TResponse>(IGuiComponent<TResponse> Inner) : IGuiComponent<TResponse>
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            var innerConstraints = this.Inner.GetConstraints();
            return innerConstraints with
            {
                MaxSize = new(innerConstraints.MinSize),
            };
        }

        /// <inheritdoc />
        public TResponse Handle(GuiEvent e, Rectangle bounds)
        {
            return this.Inner.Handle(e, bounds);
        }
    }
}
