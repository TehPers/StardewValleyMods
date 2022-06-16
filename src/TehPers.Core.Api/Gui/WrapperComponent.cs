using Microsoft.Xna.Framework;
using System;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// An extendable wrapper around an existing component.
    /// </summary>
    public abstract class WrapperComponent : IGuiComponent
    {
        /// <summary>
        /// The inner component.
        /// </summary>
        protected IGuiComponent Inner { get; }

        /// <summary>
        /// Creates a wrapper around an existing component.
        /// </summary>
        /// <param name="inner">The inner component.</param>
        protected WrapperComponent(IGuiComponent inner)
        {
            this.Inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        /// <inheritdoc />
        public virtual GuiConstraints GetConstraints()
        {
            return this.Inner.GetConstraints();
        }

        /// <inheritdoc />
        public virtual void Handle(GuiEvent e, Rectangle bounds)
        {
            this.Inner.Handle(e, bounds);
        }
    }

    /// <summary>
    /// An extendable wrapper around an existing component.
    /// </summary>
    /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
    public abstract class WrapperComponent<TResponse> : IGuiComponent<TResponse>
    {
        /// <summary>
        /// The inner component.
        /// </summary>
        protected IGuiComponent<TResponse> Inner { get; }

        /// <summary>
        /// Creates a wrapper around an existing component.
        /// </summary>
        /// <param name="inner">The inner component.</param>
        protected WrapperComponent(IGuiComponent<TResponse> inner)
        {
            this.Inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        /// <inheritdoc />
        public virtual GuiConstraints GetConstraints()
        {
            return this.Inner.GetConstraints();
        }

        /// <inheritdoc />
        public virtual TResponse Handle(GuiEvent e, Rectangle bounds)
        {
            return this.Inner.Handle(e, bounds);
        }
    }
}
