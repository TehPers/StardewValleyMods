namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// The movement behavior of the fish in the minigame.
    /// </summary>
    public enum DartBehavior
    {
        /// <summary>
        /// Maps to "mixed" vanilla behavior.
        /// </summary>
        Mixed,
        /// <summary>
        /// Maps to "dart" vanilla behavior.
        /// </summary>
        Dart,
        /// <summary>
        /// Maps to "smooth" vanilla behavior.
        /// </summary>
        Smooth,
        /// <summary>
        /// Maps to "sink" vanilla behavior.
        /// </summary>
        Sink,
        /// <summary>
        /// Maps to "floater" vanilla behavior.
        /// </summary>
        Floater,
    }
}