using StardewValley;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// The speaker portrait to show in a dialogue box, if any.
    /// </summary>
    public enum DialogueSpeakerPortrait
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
