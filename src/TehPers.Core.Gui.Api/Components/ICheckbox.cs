namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A checkbox component.
/// </summary>
public interface ICheckbox : IGuiComponent, IWithState<ICheckbox.IState, ICheckbox>
{
    /// <summary>
    /// The state of a checkbox.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Gets or sets whether the checkbox is checked.
        /// </summary>
        public bool Checked { get; set; }
    }

    /// <inheritdoc />
    public class State : IState
    {
        /// <inheritdoc />
        public bool Checked { get; set; }
    }
}
