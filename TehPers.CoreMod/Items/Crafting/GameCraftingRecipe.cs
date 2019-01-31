using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Items.Inventory;
using TehPers.CoreMod.Api.Items.Recipes;

namespace TehPers.CoreMod.Items.Crafting {
    internal class GameCraftingRecipe : CustomCraftingRecipe {
        public override int ComponentWidth => 1;
        public override int ComponentHeight => this.bigCraftable ? 2 : 1;
        public override IRecipe Recipe { get; }

        public GameCraftingRecipe(ICoreApi coreApi, string name, bool isCookingRecipe) : base(name, isCookingRecipe) {
            this.Recipe = new GameRecipe(coreApi, name, isCookingRecipe);
        }
    }
}