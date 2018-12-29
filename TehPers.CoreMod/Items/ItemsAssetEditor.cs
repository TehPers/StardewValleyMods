using StardewModdingAPI;

namespace TehPers.CoreMod.Items {
    internal class ItemsAssetEditor : IAssetEditor {

        public bool CanEdit<T>(IAssetInfo asset) {
            return ItemDelegator.CanEdit(asset);
        }

        public void Edit<T>(IAssetData asset) {
            ItemDelegator.Edit(asset);
        }
    }
}