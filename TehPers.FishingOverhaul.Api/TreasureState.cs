namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// The state of the treasure in the fishing minigame.
    /// </summary>
    public enum TreasureState
    {
        /// <summary>
        /// No treasure can be caught.
        /// </summary>
        None,
        /// <summary>
        /// Treasure can be caught, but it has not yet been caught.
        /// </summary>
        NotCaught,
        /// <summary>
        /// Treasure was caught.
        /// </summary>
        Caught,
    }
}