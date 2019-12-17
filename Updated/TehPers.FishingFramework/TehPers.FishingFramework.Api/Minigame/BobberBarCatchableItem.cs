namespace TehPers.FishingFramework.Api.Minigame
{
    /// <inheritdoc />
    public class BobberBarCatchableItem : IBobberBarCatchableItem
    {
        /// <summary>
        /// Gets or sets the y position of the item on the bobber bar.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the catch progress of the item. Progress ranges from 0.0 (0% caught) to 1.0 (100% caught).
        /// </summary>
        public double Progress { get; set; }
    }
}