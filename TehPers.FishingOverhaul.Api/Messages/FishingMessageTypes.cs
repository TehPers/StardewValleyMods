namespace TehPers.FishingOverhaul.Api.Messages
{
    public static class FishingMessageTypes
    {
        /// <summary>
        /// Associated with message type of <see cref="TrashCaughtMessage"/>.
        /// </summary>
        public const string TrashCaught = "trashCaught";

        /// <summary>
        /// Associated with message type of <see cref="SpecialCaughtMessage"/>.
        /// </summary>
        public const string SpecialCaught = "specialCaught";

        /// <summary>
        /// Associated with message type of <see cref="StartFishingMessage"/>.
        /// </summary>
        public const string StartFishing = "startFishing";

        /// <summary>
        /// Associated with message type of <see cref="FishCaughtMessage"/>.
        /// </summary>
        public const string FishCaught = "fishCaught";
    }
}