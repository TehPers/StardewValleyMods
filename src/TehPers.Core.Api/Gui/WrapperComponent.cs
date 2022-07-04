using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// An extendable wrapper around an existing component.
    /// </summary>
    internal abstract record WrapperComponent : IGuiComponent
    {
        /// <summary>
        /// The inner component.
        /// </summary>
        public abstract IGuiComponent Inner { get; }
        
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
}
