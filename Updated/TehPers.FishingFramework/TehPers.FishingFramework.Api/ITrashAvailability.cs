using StardewValley;
using SObject = StardewValley.Object;

namespace TehPers.FishingFramework.Api
{
    /// <summary>
    /// Trash which is sometimes available while fishing.
    /// </summary>
    public interface ITrashAvailability : IFishingAvailability
    {
        /// <summary>
        /// Gets the <see cref="Item.ParentSheetIndex"/> for this trash. All trash must be instances of <see cref="SObject"/>.
        /// </summary>
        int ItemIndex { get; }
    }
}