namespace TehPers.FishingFramework.Api.Minigame
{
    /// <summary>
    /// An item which can be caught during the fishing mingame.
    /// </summary>
    public interface IBobberBarCatchableItem
    {
        /// <summary>
        /// Gets or sets the y position of the item on the bobber bar.
        /// </summary>
        int Position { get; set; }

        /// <summary>
        /// Gets or sets the catch progress of the item. Progress ranges from 0.0 (0% caught) to 1.0 (100% caught).
        /// </summary>
        double Progress { get; set; }
    }
}