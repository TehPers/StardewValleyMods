using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// A possible catch from fishing. This may or may not be a fish.
    /// </summary>
    public record PossibleCatch(NamespacedKey ItemKey, PossibleCatch.Type CatchType)
    {
        public enum Type
        {
            Fish,
            Trash,
            Special,
        }
    }
}