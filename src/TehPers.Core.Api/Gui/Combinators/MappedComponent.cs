using Microsoft.Xna.Framework;
using System;

namespace TehPers.Core.Api.Gui.Combinators
{
    /// <summary>
    /// Maps the response from a component.
    /// </summary>
    /// <typeparam name="T1">The original response type.</typeparam>
    /// <typeparam name="T2">The new response type.</typeparam>
    /// <param name="Inner">The inner component.</param>
    /// <param name="Map">A function which maps the original response to a new response.</param>
    public record MappedComponent<T1, T2>
        (IGuiComponent<T1> Inner, Func<T1, T2> Map) : IGuiComponent<T2>
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            return this.Inner.GetConstraints();
        }

        /// <inheritdoc />
        public T2 Handle(GuiEvent e, Rectangle bounds)
        {
            var response = this.Inner.Handle(e, bounds);
            return this.Map(response);
        }
    }
}
