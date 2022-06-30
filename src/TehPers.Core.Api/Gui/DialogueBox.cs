using Microsoft.Xna.Framework;
using StardewValley;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A dialogue box.
    /// </summary>
    public class DialogueBox : IGuiComponent
    {
        /// <summary>
        /// The portrait to show, if any.
        /// </summary>
        public SpeakerPortrait Speaker { get; init; } = SpeakerPortrait.None;

        /// <summary>
        /// Whether to only draw the dialogue box itself.
        /// </summary>
        public bool DrawOnlyBox { get; init; } = true;

        /// <summary>
        /// The message to show in the dialogue box.
        /// </summary>
        public string? Message { get; init; } = null;

        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            return new()
            {
                MinSize = new(64 * 3, 64 * 3),
            };
        }

        /// <inheritdoc />
        public void Handle(GuiEvent e, Rectangle bounds)
        {
            e.Draw(
                batch =>
                {
                    var prevBatch = Game1.spriteBatch;
                    Game1.spriteBatch = batch;
                    Game1.drawDialogueBox(
                        bounds.X,
                        bounds.Y,
                        bounds.Width,
                        bounds.Height,
                        this.Speaker is not SpeakerPortrait.None,
                        this.DrawOnlyBox,
                        this.Message,
                        this.Speaker is SpeakerPortrait.ObjectPortrait
                    );
                    Game1.spriteBatch = prevBatch;
                }
            );
        }

        /// <summary>
        /// The speaker portrait to show, if any.
        /// </summary>
        public enum SpeakerPortrait
        {
            /// <summary>
            /// No portrait should be shown.
            /// </summary>
            None,

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
}
