namespace TehPers.FishingOverhaul.Effects
{
    /// <summary>
    /// The type of chance to modify.
    /// </summary>
    public enum ModifyChanceType
    {
        /// <summary>
        /// Chance of finding a fish.
        /// </summary>
        Fish,

        /// <summary>
        /// Minimum chance of finding a fish.
        /// </summary>
        MinFish,

        /// <summary>
        /// Maximum chance of finding a fish.
        /// </summary>
        MaxFish,

        /// <summary>
        /// Chance of finding a treasure chest.
        /// </summary>
        Treasure,

        /// <summary>
        /// Minimum chance of finding a treasure chest.
        /// </summary>
        MinTreasure,

        /// <summary>
        /// Maximum chance of finding a treasure chest.
        /// </summary>
        MaxTreasure,
    }
}
