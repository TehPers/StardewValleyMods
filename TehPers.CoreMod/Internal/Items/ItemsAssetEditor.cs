using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using TehPers.CoreMod.Api.Items;

namespace TehPers.CoreMod.Internal.Items {
    internal class ItemsAssetEditor : IAssetEditor {

        public bool CanEdit<T>(IAssetInfo asset) {
            return ItemDelegator.CanEdit(asset);
        }

        public void Edit<T>(IAssetData asset) {
            ItemDelegator.Edit(asset);
        }
    }
}