using System.Collections.Generic;
using System.Linq;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items.Inventory;

namespace TehPers.CoreMod.Api.Items.Recipes {
    public class ModRecipe : SimpleRecipe {
        public override IEnumerable<IIngredient> Ingredients { get; }
        public override IEnumerable<IItemResult> Results { get; }
        public override ISprite Sprite { get; }

        public ModRecipe(ISprite sprite, IItemResult result, params IIngredient[] ingredients) : this(sprite, result.Yield(), ingredients?.AsEnumerable()) { }
        public ModRecipe(ISprite sprite, IItemResult result, IEnumerable<IIngredient> ingredients) : this(sprite, result.Yield(), ingredients?.AsEnumerable()) { }
        public ModRecipe(ISprite sprite, IEnumerable<IItemResult> results, params IIngredient[] ingredients) : this(sprite, results, ingredients?.AsEnumerable()) { }
        public ModRecipe(ISprite sprite, IEnumerable<IItemResult> results, IEnumerable<IIngredient> ingredients) {
            this.Sprite = sprite;
            this.Results = results;
            this.Ingredients = ingredients;
        }
    }
}