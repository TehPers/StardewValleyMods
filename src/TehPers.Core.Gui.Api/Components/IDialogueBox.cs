using StardewValley;

namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A dialogue box.
/// </summary>
public interface IDialogueBox : IGuiComponent
{
    /// <summary>
    /// Sets the portait to show for the speaker, if any.
    /// </summary>
    /// <param name="portrait">The new portrait, if any.</param>
    /// <returns>The resulting component.</returns>
    IDialogueBox WithSpeakerPortrait(Portrait? portrait);

    /// <summary>
    /// Sets whether to draw only the dialogue box itself.
    /// </summary>
    /// <param name="drawOnlyBox">Whether to draw only the dialogue box.</param>
    /// <returns>The resulting component.</returns>
    IDialogueBox WithDrawOnlyBox(bool drawOnlyBox);

    /// <summary>
    /// Sets the message to show in the dialogue box, if any.
    /// </summary>
    /// <param name="message">The new message, if any.</param>
    /// <returns>The resulting component.</returns>
    IDialogueBox WithMessage(string? message);

    /// <summary>
    /// The speaker portrait to show in a dialogue box, if any.
    /// </summary>
    public enum Portrait
    {
        /// <summary>
        /// The portrait for <see cref="Game1.currentSpeaker"/> should be shown.
        /// </summary>
        CurrentSpeakerPortrait,

        /// <summary>
        /// The portrait for <see cref="Game1.objectDialoguePortraitPerson"/> should be shown.
        /// </summary>
        ObjectPortrait,
    }
}
