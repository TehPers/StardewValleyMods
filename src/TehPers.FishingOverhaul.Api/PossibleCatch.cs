using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// A possible catch from fishing. This may or may not be a fish.
    /// </summary>
    public abstract record PossibleCatch
    {
        private PossibleCatch() { }

        /// <summary>
        /// A fish catch.
        /// </summary>
        public sealed record Fish(FishEntry Entry) : PossibleCatch;

        /// <summary>
        /// A trash catch.
        /// </summary>
        public sealed record Trash(TrashEntry Entry) : PossibleCatch;
    }
}
