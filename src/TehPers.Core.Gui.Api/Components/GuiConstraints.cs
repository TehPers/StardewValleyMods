namespace TehPers.Core.Gui.Api.Components;

/// <inheritdoc />
/// <param name="MinSize">The minimum size of this component.</param>
/// <param name="MaxSize">The maximum size of this component, if any.</param>
public record GuiConstraints(
    IGuiSize? MinSize = null,
    IPartialGuiSize? MaxSize = null
) : IGuiConstraints
{
    /// <inheritdoc />
    public IGuiSize MinSize { get; init; } = MinSize ?? GuiSize.Zero;

    /// <inheritdoc />
    public IPartialGuiSize MaxSize { get; init; } = MaxSize ?? PartialGuiSize.Empty;

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
