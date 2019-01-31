using StardewValley;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Items.Recipes;

namespace TehPers.CoreMod.Items.Recipes {
    internal class ModIngredient : IIngredient {
        private readonly IItemApi _itemApi;
        private readonly ItemKey _key;

        public int Quantity { get; }
        public ISprite Sprite { get; }

        public ModIngredient(IItemApi itemApi, ItemKey key, int quantity, ISprite sprite) {
            this._itemApi = itemApi;
            this._key = key;
            this.Quantity = quantity;
            this.Sprite = sprite;
        }

        public bool Matches(Item item) {
            return this._itemApi.IsInstanceOf(this._key, item);
        }

        public string GetDisplayName() {
            return this._itemApi.TryCreate(this._key, out Item item) ? item.DisplayName : "Invalid ingredient";
        }
    }
}