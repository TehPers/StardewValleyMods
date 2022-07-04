namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Shrinks a GUI component to its minimum size.
    /// </summary>
    internal record ShrinkComponent : WrapperComponent
    {
        public override IGuiComponent Inner { get; }

        /// <summary>
        /// Shrinks a GUI component to its minimum size.
        /// </summary>
        /// <param name="inner">The inner component.</param>
        public ShrinkComponent(IGuiComponent inner)
        {
            this.Inner = inner;
        }

        /// <inheritdoc />
        public override GuiConstraints GetConstraints()
        {
            var innerConstraints = base.GetConstraints();
            return innerConstraints with
            {
                MaxSize = new(innerConstraints.MinSize),
            };
        }
    }
}
