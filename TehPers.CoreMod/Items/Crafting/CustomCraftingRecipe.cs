using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using TehPers.CoreMod.Api.Items.Inventory;
using TehPers.CoreMod.Api.Items.Recipes;

namespace TehPers.CoreMod.Items.Crafting {
    internal abstract class CustomCraftingRecipe : CraftingRecipe {
        public abstract int ComponentWidth { get; }
        public abstract int ComponentHeight { get; }
        public abstract IRecipe Recipe { get; }

        protected CustomCraftingRecipe(string name, bool isCookingRecipe) : base(name, isCookingRecipe) { }

        public Texture2D GetTexture() {
            return this.Recipe.Sprite.ParentSheet.TrackedTexture.CurrentTexture;
        }

        public Rectangle GetSourceRectangle() {
            return new Rectangle(this.Recipe.Sprite.U, this.Recipe.Sprite.V, this.Recipe.Sprite.Width, this.Recipe.Sprite.Height);
        }

        public bool TryCraft(IInventory inventory, out IEnumerable<Item> results) {
            return this.Recipe.TryCraft(inventory, out results);
        }
    }
}