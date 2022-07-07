using Microsoft.Xna.Framework;
using StardewValley;

namespace TehPers.Core.Api.Gui.Components
{
    /// <summary>
    /// A dialogue box.
    /// </summary>
    internal record DialogueBox : IGuiComponent
    {
        /// <summary>
        /// The portrait to show, if any.
        /// </summary>
        public DialogueSpeakerPortrait? Speaker { get; init; } = null;

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
                        this.Speaker is not null,
                        this.DrawOnlyBox,
                        this.Message,
                        this.Speaker is DialogueSpeakerPortrait.ObjectPortrait
                    );
                    Game1.spriteBatch = prevBatch;
                }
            );
        }
    }
}
