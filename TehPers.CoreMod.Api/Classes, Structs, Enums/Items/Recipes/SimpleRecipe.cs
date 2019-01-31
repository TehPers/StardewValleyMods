using System.Collections.Generic;
using System.Linq;
using StardewValley;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items.Inventory;

namespace TehPers.CoreMod.Api.Items.Recipes {
    public abstract class SimpleRecipe : IRecipe {
        public abstract IEnumerable<IIngredient> Ingredients { get; }
        public abstract IEnumerable<IItemResult> Results { get; }
        public abstract ISprite Sprite { get; }
        
        public string GetDisplayName() {
            return this.Results.FirstOrDefault() is IItemResult firstResult && firstResult.TryCreateOne(out Item item) ? item.DisplayName : "Invalid Recipe";
        }

        public string GetDescription() {
            return this.Results.FirstOrDefault() is IItemResult firstResult && firstResult.TryCreateOne(out Item item) ? item.getDescription() : "Invalid recipe";
        }

        public bool TryCraft(IInventory inventory, out IEnumerable<Item> results) {
            // Create results to make sure it's possible
            List<Item> resultItems = new List<Item>();
            foreach (IItemResult result in this.Results) {
                for (int n = 0; n < result.Quantity; n++) {
                    if (!result.TryCreateOne(out Item item)) {
                        results = default;
                        return false;
                    }

                    resultItems.Add(item);
                }
            }

            // Try to remove the ingredients from the inventory
            if (inventory.TryRemoveAll(this.Ingredients, out _)) {
                // The items were crafted successfully
                results = resultItems;
                return true;
            }

            results = default;
            return false;
        }
    }
}