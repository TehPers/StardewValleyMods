using System.Collections.Generic;
using StardewValley;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items.Inventory;

namespace TehPers.CoreMod.Api.Items.Recipes {
    public interface IRecipe {
        /// <summary>The ingredients required to create the result.</summary>
        IEnumerable<IIngredient> Ingredients { get; }

        /// <summary>The result of crafting this recipe.</summary>
        IEnumerable<IItemResult> Results { get; }

        /// <summary>The sprite that will be displayed when this recipe is drawn in the crafting page.</summary>
        ISprite Sprite { get; }

        /// <summary>Gets the name displayed by this recipe when it appears in the crafting page.</summary>
        /// <returns>The display name of this recipe.</returns>
        string GetDisplayName();

        /// <summary>Gets the description displayed by this recipe when it appears in the crafting page.</summary>
        /// <returns>The description of this recipe.</returns>
        string GetDescription();

        /// <summary>Tries to craft this recipe.</summary>
        /// <param name="inventory">The inventory to pull items from.</param>
        /// <param name="results">The results from crafting this recipe.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryCraft(IInventory inventory, out IEnumerable<Item> results);
    }
}