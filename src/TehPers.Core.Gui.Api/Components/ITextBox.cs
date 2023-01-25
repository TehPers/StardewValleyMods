using StardewModdingAPI;

namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A standard text box.
/// </summary>
public interface ITextBox : IGuiComponent, IWithState<ITextInput.IState, ITextBox>
{
    /// <summary>
    /// Sets the input helper to use for tracking input.
    /// </summary>
    /// <param name="inputHelper">The input helper.</param>
    /// <returns>The resulting component</returns>
    ITextBox WithInputHelper(IInputHelper inputHelper);
}
