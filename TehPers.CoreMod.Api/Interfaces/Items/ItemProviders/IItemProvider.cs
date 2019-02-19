using System.Collections.Generic;

namespace TehPers.CoreMod.Api.Items.ItemProviders {
    public interface IItemProvider : IItemCreator, IItemComparer, IItemDrawingProvider {
        /// <summary>All the indexes this provider uses, regardless of the type of item it is. Teh's Core Mod uses this list to avoid assigning conflicting indexes whenever possible.</summary>
        IEnumerable<int> ReservedIndexes { get; }

        /// <summary>Invalidates any assets used by the items registered by this provider. This is called after each item is assigned an index, which occurs when a save is loaded or when connecting to a multiplayer game.</summary>
        void InvalidateAssets();
    }
}