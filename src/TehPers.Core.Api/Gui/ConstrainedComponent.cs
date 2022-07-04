using System;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Adds constraints to a component's size. This cannot be used to remove constraints.
    /// </summary>
    internal record ConstrainedComponent : WrapperComponent
    {
        protected override IGuiComponent Inner { get; }

        /// <summary>
        /// The additional minimum size constraints for the component. This cannot make the
        /// component smaller than its previous minimum size.
        /// </summary>
        public PartialGuiSize MinSize { get; init; } = PartialGuiSize.Empty;

        /// <summary>
        /// The additional maximum size constraints for the component. This cannot make the
        /// component larger than its previous maximum size.
        /// </summary>
        public PartialGuiSize MaxSize { get; init; } = PartialGuiSize.Empty;

        /// <summary>
        /// Adds constraints to a component's size. This cannot be used to remove constraints.
        /// </summary>
        /// <param name="inner">The inner component.</param>
        public ConstrainedComponent(IGuiComponent inner)
        {
            this.Inner = inner;
        }

        /// <inheritdoc />
        public override GuiConstraints GetConstraints()
        {
            var innerConstraints = base.GetConstraints();
            var minWidth = this.MinSize.Width switch
            {
                { } w => Math.Max(w, innerConstraints.MinSize.Width),
                _ => innerConstraints.MinSize.Width,
            };
            var minHeight = this.MinSize.Height switch
            {
                { } h => Math.Max(h, innerConstraints.MinSize.Height),
                _ => innerConstraints.MinSize.Height,
            };
            var maxWidth = (innerConstraints.MaxSize.Width, this.MaxSize.Width) switch
            {
                ({ } w1, { } w2) => Math.Min(w1, w2),
                (null, var w) => w,
                var (w, _) => w,
            };
            var maxHeight = (innerConstraints.MaxSize.Height, this.MaxSize.Height) switch
            {
                ({ } h1, { } h2) => Math.Min(h1, h2),
                (null, var h) => h,
                var (h, _) => h,
            };
            return innerConstraints with
            {
                MinSize = new(minWidth, minHeight),
                MaxSize = new(maxWidth, maxHeight),
            };
        }
    }
}
