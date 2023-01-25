namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// Constraints on how an <see cref="IGuiComponent"/> should be rendered.
/// </summary>
public interface IGuiConstraints
{
    /// <summary>
    /// The minimum size of this component. The component may be given an area with
    /// less size than this when being drawn, but it may not be rendered correctly if so. For
    /// example, it might get cut off or overlap into another component.
    /// </summary>
    public IGuiSize MinSize { get; }

    /// <summary>
    /// The maximum size of this component, if any. The component may be given an area with
    /// more size than this when being drawn, but it may not be rendered correctly if so. For
    /// example, there might be unexpected extra space around it or it might be stretched.
    /// </summary>
    public IPartialGuiSize MaxSize { get; }

    /// <summary>
    /// Deconstructs the constraints into its components.
    /// </summary>
    /// <param name="minSize">The minimum size constraint.</param>
    /// <param name="maxSize">The maximum size constraint.</param>
    public void Deconstruct(out IGuiSize minSize, out IPartialGuiSize maxSize)
    {
        minSize = this.MinSize;
        maxSize = this.MaxSize;
    }
}
