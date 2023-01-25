namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// Adds additional constraints to a component's size.
/// </summary>
public interface IConstrainer : IGuiComponent, IWithInner<IConstrainer>
{
    /// <summary>
    /// Sets the additional minimum size constraints for the component. This cannot make the
    /// component smaller than its previous minimum size.
    /// </summary>
    /// <param name="minSize">The new minimum size (if the inner component will fit).</param>
    /// <returns>The resulting component.</returns>
    IConstrainer WithMinSize(IPartialGuiSize minSize);

    /// <summary>
    /// Sets the additional maximum size constraints for the component. This cannot make the
    /// component larger than its previous maximum size.
    /// </summary>
    /// <param name="maxSize">The new maximum size (if the inner component will fit).</param>
    /// <returns>The resulting component.</returns>
    IConstrainer WithMaxSize(IPartialGuiSize maxSize);
}
