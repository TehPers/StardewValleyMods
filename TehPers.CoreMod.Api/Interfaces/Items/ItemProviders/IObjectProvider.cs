namespace TehPers.CoreMod.Api.Items.ItemProviders {
    public interface IObjectProvider : IItemProvider {
        /// <summary>Registers a new simple item with the game. It will be treated similar to items in "Maps/springobjects".</summary>
        /// <param name="localKey">The local key for this item, unique within your mod.</param>
        /// <param name="objectManager">The object's manager.</param>
        /// <returns>The key for the item once registered.</returns>
        ItemKey Register(string localKey, IModObject objectManager);

        /// <summary>Registers a new simple item with the game. It will be treated similar to items in "Maps/springobjects".</summary>
        /// <param name="key">The key for this item, unique within all items registered through Teh's Core Mod.</param>
        /// <param name="objectManager">The object's manager.</param>
        /// <returns>The key for the item once registered.</returns>
        void Register(ItemKey key, IModObject objectManager);
    }
}