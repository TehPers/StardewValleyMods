namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// Aligns a component. This removes any maximum size constraints on aligned axes.
/// </summary>
public interface IAligner : IGuiComponent, IWithInner<IAligner>
{
    /// <summary>
    /// Sets the horizontal alignment of this component.
    /// </summary>
    /// <param name="alignment">The new horizontal alignment, if any.</param>
    /// <returns>The resulting component.</returns>
    IAligner WithHorizontalAlignment(HorizontalAlignment alignment);

    /// <summary>
    /// Sets the vertical alignment of this component.
    /// </summary>
    /// <param name="alignment">The new vertical alignment, if any.</param>
    /// <returns>The resulting component.</returns>
    IAligner WithVerticalAlignment(VerticalAlignment alignment);
}
