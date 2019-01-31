using StardewValley;

namespace TehPers.CoreMod.Api.Items.Inventory {
    public interface IItemResult {
        /// <summary>The number of times to create the result.</summary>
        int Quantity { get; }

        /// <summary>Tries to create an instance of the result.</summary>
        /// <param name="result">The item constructed from this result.</param>
        /// <returns>True if successfully created, false otherwise.</returns>
        bool TryCreateOne(out Item result);
    }
}