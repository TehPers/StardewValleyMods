using TehPers.Core.Api.Gui.Components;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Constraints on how an <see cref="IGuiComponent"/> should be rendered.
    /// </summary>
    public record GuiConstraints
    {
        /// <summary>
        /// The minimum size of this component. The component may be given an area with
        /// less size than this when being drawn, but it may not be rendered correctly if so. For
        /// example, it might get cut off or overlap into another component.
        /// </summary>
        public GuiSize MinSize { get; init; } = GuiSize.Zero;

        /// <summary>
        /// The maximum size of this component, if any. The component may be given an area with
        /// more size than this when being drawn, but it may not be rendered correctly if so. For
        /// example, there might be unexpected extra space around it or it might be stretched.
        /// </summary>
        public PartialGuiSize MaxSize { get; init; } = PartialGuiSize.Empty;
    }
}
