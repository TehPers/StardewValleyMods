using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Items.Inventory;
using TehPers.CoreMod.Api.Items.Recipes;
using TehPers.CoreMod.Items.Inventory;
using SObject = StardewValley.Object;

namespace TehPers.CoreMod.Items.Crafting {
    internal class CraftingManager {
        private static readonly ConditionalWeakTable<CraftingPage, CraftingPageData> _extraCraftingPageData = new ConditionalWeakTable<CraftingPage, CraftingPageData>();
        private static CraftingManager _instance;

        private readonly IMod _coreMod;
        private readonly ItemDelegator _itemDelegator;
        private readonly Dictionary<string, IRecipe> _addedRecipes = new Dictionary<string, IRecipe>();

        private CraftingManager(IMod coreMod, ItemDelegator itemDelegator) {
            this._coreMod = coreMod;
            this._itemDelegator = itemDelegator;

            // Patches
            this.Patch();
        }

        public void Initialize() {
            this._coreMod.Helper.Content.AssetEditors.Add(new CraftingRecipeAssetEditor(this));
        }

        public IEnumerable<string> GetAddedRecipes() {
            return this._addedRecipes.Keys;
        }

        private void Patch() {
            HarmonyInstance harmony = HarmonyInstance.Create("TehPers.CoreMod.CraftingManager");

            // CraftingPage.layoutRecipes(List<string> playerRecipes)
            MethodBase target = typeof(CraftingPage).GetMethod("layoutRecipes", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingPage_layoutRecipes_Prefix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingPage.clickCraftingRecipe(ClickableTextureComponent c, bool playSound)
            target = typeof(CraftingPage).GetMethod("clickCraftingRecipe", BindingFlags.NonPublic | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingPage_clickCraftingRecipe_Prefix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingPage.readyToClose()
            target = typeof(CraftingPage).GetMethod(nameof(CraftingPage.readyToClose), BindingFlags.Public | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingPage_readyToClose_Prefix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingPage.receiveKeyPress(Keys key)
            target = typeof(CraftingPage).GetMethod(nameof(CraftingPage.receiveKeyPress), BindingFlags.Public | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingPage_receiveKeyPress_Prefix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingPage.draw(SpriteBatch b)
            target = typeof(CraftingPage).GetMethod(nameof(CraftingPage.draw), BindingFlags.Public | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingPage_draw_Prefix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingRecipe.createItem()
            target = typeof(CraftingRecipe).GetMethod(nameof(CraftingRecipe.createItem), BindingFlags.Public | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingRecipe_createItem), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingRecipe.getNumberOfIngredients()
            target = typeof(CraftingRecipe).GetMethod(nameof(CraftingRecipe.getNumberOfIngredients), BindingFlags.Public | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingRecipe_getNumberOfIngredients_Prefix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingRecipe.drawRecipeDescription(SpriteBatch b, Vector2 position, int width)
            target = typeof(CraftingRecipe).GetMethod(nameof(CraftingRecipe.drawRecipeDescription), BindingFlags.Public | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingRecipe_drawRecipeDescription_Prefix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));
        }

        public string AddRecipe(IRecipe recipe) {
            string key = $"tcm_recipe{this._addedRecipes.Count}";
            this._addedRecipes.Add(key, recipe);
            this._coreMod.Helper.Content.InvalidateCache("Data/CraftingRecipes");
            CraftingRecipe.InitShared();
            return key;
        }

        public static CraftingManager GetCraftingManager(IMod mod, ItemDelegator itemDelegator) {
            if (CraftingManager._instance == null) {
                CraftingManager._instance = new CraftingManager(mod, itemDelegator);
            }

            return CraftingManager._instance;
        }

        private static CustomCraftingRecipe CreateRecipe(string name, bool cooking) {
            if (CraftingManager._instance._addedRecipes.TryGetValue(name, out IRecipe modRecipe)) {
                return new ModCraftingRecipe(name, modRecipe, cooking);
            }

            ICoreApi coreApi = (CraftingManager._instance._coreMod.GetApi() as ICoreApiFactory)?.GetApi(CraftingManager._instance._coreMod);
            return new GameCraftingRecipe(coreApi, name, cooking);
        }

        private static CraftingPageData GetExtraData(CraftingPage page) {
            if (!CraftingManager._extraCraftingPageData.TryGetValue(page, out CraftingPageData extraData)) {
                extraData = new CraftingPageData(page);
                CraftingManager._extraCraftingPageData.Add(page, extraData);
            }

            return extraData;
        }

        #region CraftingPage Patches
        private static bool CraftingPage_layoutRecipes_Prefix(CraftingPage __instance, List<string> playerRecipes, bool ___cooking) {
            // Just run the original method if no recipes were added
            if (!CraftingManager._instance._addedRecipes.Any()) {
                return true;
            }

            // Layout the crafting pages

            int startX = __instance.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth - 16;
            int startY = __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - 16;
            const int pageWidth = 10;
            const int pageHeight = 4;
            const int perPage = pageWidth * pageHeight;
            const int recipePadding = 8;

            // Create a queue of all the recipe objects to add
            Queue<CustomCraftingRecipe> recipesRemaining = new Queue<CustomCraftingRecipe>(playerRecipes.Select(name => CraftingManager.CreateRecipe(name, ___cooking)).OrderByDescending(r => Math.Max(r.ComponentWidth, r.ComponentHeight)));

            // For each recipe, create a clickable texture component and assign it a page and location within the page
            Dictionary<CraftingPageLocation, (CustomCraftingRecipe recipe, ClickableTextureComponent component)> components = new Dictionary<CraftingPageLocation, (CustomCraftingRecipe recipe, ClickableTextureComponent component)>();
            while (recipesRemaining.Any()) {
                CustomCraftingRecipe recipe = recipesRemaining.Dequeue();

                // Find the first available spot
                CraftingPageLocation curLocation = new CraftingPageLocation(0, 0, 0);
                while (!IsValidSpaceFor(recipe, curLocation)) {
                    curLocation = curLocation.NextLocation(pageWidth, pageHeight);
                }

                (int page, int y, int x) = curLocation;

                Rectangle bounds = new Rectangle(startX + x * (64 + recipePadding), startY + y * (64 + recipePadding), 64, recipe.bigCraftable ? 128 : 64);
                string hoverText = !___cooking || Game1.player.cookingRecipes.ContainsKey(recipe.name) ? "" : "ghosted";
                Texture2D texture = recipe.GetTexture();
                Rectangle sourceRect = recipe.GetSourceRectangle();

                // Create the component
                ClickableTextureComponent component = new ClickableTextureComponent("", bounds, null, hoverText, texture, sourceRect, 4f) {
                    myID = 200 + page * perPage + y * pageWidth + x,
                    fullyImmutable = true,
                    region = 8000
                };

                // Add it to the dictionary in each slot it takes up
                for (int slotX = x; slotX < x + recipe.ComponentWidth; slotX++) {
                    for (int slotY = y; slotY < y + recipe.ComponentHeight; slotY++) {
                        components.Add(new CraftingPageLocation(page, slotY, slotX), (recipe, component));
                    }
                }
            }

            // Assign each component's neighbors and add them to the crafting recipe pages
            Dictionary<ClickableTextureComponent, CraftingRecipe>[] pages = Enumerable.Range(0, components.Keys.Max(location => location.Page) + 1).Select(_ => new Dictionary<ClickableTextureComponent, CraftingRecipe>()).ToArray();
            foreach (((int page, int y, int x), (CustomCraftingRecipe recipe, ClickableTextureComponent component)) in components) {
                // Add the current component and recipe to the appropriate page
                // This doesn't use .Add because big craftables will be iterated over multiple times during this process
                pages[page][component] = recipe;

                // Left neighbor
                if (components.TryGetValue(new CraftingPageLocation(page, y, x - 1), out (CustomCraftingRecipe recipe, ClickableTextureComponent component) leftNeighbor)) {
                    if (leftNeighbor.component != component) {
                        component.leftNeighborID = leftNeighbor.component.myID;
                    }
                } else {
                    component.leftNeighborID = -1;
                }

                // Right neighbor
                if (components.TryGetValue(new CraftingPageLocation(page, y, x + 1), out (CustomCraftingRecipe recipe, ClickableTextureComponent component) rightNeighbor)) {
                    if (rightNeighbor.component != component) {
                        component.rightNeighborID = rightNeighbor.component.myID;
                    }
                } else {
                    component.rightNeighborID = y >= 2 || page == 0 ? 89 : 88;
                }

                // Up neighbor
                if (components.TryGetValue(new CraftingPageLocation(page, y - 1, x), out (CustomCraftingRecipe recipe, ClickableTextureComponent component) upNeighbor)) {
                    if (upNeighbor.component != component) {
                        component.upNeighborID = upNeighbor.component.myID;
                    }
                } else {
                    component.upNeighborID = 12344;
                }

                // Down neighbor
                if (components.TryGetValue(new CraftingPageLocation(page, y + 1, x), out (CustomCraftingRecipe recipe, ClickableTextureComponent component) downNeighbor)) {
                    if (downNeighbor.component != component) {
                        component.downNeighborID = downNeighbor.component.myID;
                    }
                } else {
                    component.downNeighborID = x;
                }
            }

            // Update pages on the instance
            __instance.pagesOfCraftingRecipes = pages.ToList();

            // Prevent the original method from executing
            return false;

            bool IsValidSpaceFor(CustomCraftingRecipe recipe, CraftingPageLocation location) {
                // For each row this recipe will take up
                for (int y = location.Y; y < location.Y + recipe.ComponentHeight; y++) {
                    if (y >= pageHeight) {
                        return false;
                    }

                    // For each space on the row this recipe will take up
                    for (int x = location.X; x < location.X + recipe.ComponentWidth; x++) {
                        if (x >= pageWidth) {
                            return false;
                        }

                        // Check if the space is available
                        if (components.ContainsKey(new CraftingPageLocation(location.Page, y, x))) {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        private static bool CraftingPage_clickCraftingRecipe_Prefix(CraftingPage __instance, ClickableTextureComponent c, bool playSound, int ___currentCraftingPage, bool ___cooking) {
            if (!__instance.pagesOfCraftingRecipes[___currentCraftingPage].TryGetValue(c, out CraftingRecipe baseRecipe)) {
                return true;
            }

            if (!(baseRecipe is CustomCraftingRecipe customRecipe)) {
                return true;
            }

            // Try to get all the extra data for the crafting page
            CraftingPageData extraData = CraftingManager.GetExtraData(__instance);

            // Check if the result can be held
            if (extraData.HeldItems.Any()) {
                // Try to create each result
                IEnumerable<(bool success, Item item)> craftResults = customRecipe.Recipe.Results.Select(r => r.TryCreateOne(out Item item) ? (true, item) : (false, null));

                // Make sure all results can be successfully created and held
                if (craftResults.Any(r => !r.success || extraData.HeldItems.Any(heldItem => heldItem.canStackWith(r.item) && heldItem.Stack + r.item.Stack < heldItem.maximumStackSize()))) {
                    return false;
                }
            }

            // Try to craft the recipe
            if (!customRecipe.TryCraft(new FarmerInventory(Game1.player), out IEnumerable<Item> results)) {
                return false;
            }

            // Play the crafting sound
            if (playSound) {
                Game1.playSound("coin");
            }

            // Get all results
            foreach (Item result in results) {
                // Update stats and achievements
                if (___cooking) {
                    // Notify the player that the item was cooked
                    Game1.player.cookedRecipe(result.ParentSheetIndex);

                    // Check for achievements
                    Game1.stats.checkForCookingAchievements();
                } else {
                    // Update times crafted
                    if (Game1.player.craftingRecipes.TryGetValue(customRecipe.name, out int timesCrafted)) {
                        Game1.player.craftingRecipes[customRecipe.name] = timesCrafted + customRecipe.numberProducedPerCraft;
                    }

                    // Check for achievements
                    Game1.stats.checkForCraftingAchievements();
                }

                if (Game1.options.gamepadControls && Game1.player.couldInventoryAcceptThisItem(result)) {
                    // Add it to the player's inventory if they're using a gamepad
                    Game1.player.addItemToInventoryBool(result);
                } else {
                    // Grab the item
                    extraData.AddHeldItem(result);
                }
            }

            return false;
        }

        private static bool CraftingPage_readyToClose_Prefix(CraftingPage __instance, ref bool __result) {
            __result = CraftingManager.GetExtraData(__instance).HeldItems.Any();
            return false;
        }

        private static void CraftingPage_receiveKeyPress_Prefix(CraftingPage __instance, Keys key, ref Item ___heldItem) {
            // Get extra data about this crafting page
            CraftingPageData extraData = CraftingManager.GetExtraData(__instance);

            // Check if this should remove the held item
            if (key != Keys.Delete) {
                return;
            }

            int removed = extraData.HeldItems.RemoveAll(item => {
                // Check if the item can be trashed
                if (!item.canBeTrashed()) {
                    // Don't remove the item
                    return false;
                }

                // Remove it from the special items list if it's a special item
                if (item is SObject) {
                    Game1.player.specialItems.Remove(item.ParentSheetIndex);
                }

                // Remove the item
                return true;
            });

            // Play the trashcan sound if any items were removed
            if (removed > 0) {
                Game1.playSound("trashcan");
            }

            // Make sure the original method doesn't remove any items as well
            ___heldItem = null;
        }

        private static bool CraftingPage_draw_Prefix(CraftingPage __instance, SpriteBatch b, bool ___cooking, int ___currentCraftingPage, Item ___hoverItem, string ___hoverText, string ___hoverTitle, CraftingRecipe ___hoverRecipe, Item ___lastCookingHover) {
            // Get extra data about this crafting page
            CraftingPageData extraData = CraftingManager.GetExtraData(__instance);
            bool isHoldingItem = extraData.HeldItems.Any();

            // Draw the background for cooking
            if (___cooking) {
                Game1.drawDialogueBox(__instance.xPositionOnScreen, __instance.yPositionOnScreen, __instance.width, __instance.height, false, true);
            }

            // Draw the separator
            DrawHorizontalPartition(__instance.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256);

            // Draw the inventory
            __instance.inventory.draw(b);

            // Draw the trashcan
            if (__instance.trashCan != null) {
                __instance.trashCan.draw(b);
                b.Draw(Game1.mouseCursors, new Vector2(__instance.trashCan.bounds.X + 60, __instance.trashCan.bounds.Y + 40), new Rectangle(686, 256, 18, 10), Color.White, __instance.trashCanLidRotation, new Vector2(16f, 10f), 4f, SpriteEffects.None, 0.86f);
            }

            // Draw each recipe
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
            foreach (ClickableTextureComponent key in __instance.pagesOfCraftingRecipes[___currentCraftingPage].Keys) {
                // Draw the component
                if (key.hoverText.Equals("ghosted")) {
                    // Unknown recipe
                    key.draw(b, Color.Black * 0.35f, 0.89f);
                } else if (!__instance.pagesOfCraftingRecipes[___currentCraftingPage][key].doesFarmerHaveIngredientsInInventory(___cooking ? (Game1.currentLocation as FarmHouse)?.fridge.Value.items : null)) {
                    // Can't be crafted, but known
                    key.draw(b, Color.LightGray * 0.4f, 0.89f);
                } else {
                    // Can be crafted
                    key.draw(b);

                    // Show how many are crafted
                    if (__instance.pagesOfCraftingRecipes[___currentCraftingPage][key].numberProducedPerCraft > 1) {
                        NumberSprite.draw(__instance.pagesOfCraftingRecipes[___currentCraftingPage][key].numberProducedPerCraft, b, new Vector2(key.bounds.X + 64 - 2, key.bounds.Y + 64 - 2), Color.Red, (float) (0.5 * (key.scale / 4.0)), 0.97f, 1f, 0);
                    }
                }
            }

            // Draw the held items and tooltip
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            // Draw the tooltip
            if (___hoverItem != null) {
                IClickableMenu.drawToolTip(b, ___hoverText, ___hoverTitle, ___hoverItem, isHoldingItem);
            } else if (!string.IsNullOrEmpty(___hoverText)) {
                IClickableMenu.drawHoverText(b, ___hoverText, Game1.smallFont, isHoldingItem ? 64 : 0, isHoldingItem ? 64 : 0);
            }

            for (int i = Math.Min(3, extraData.HeldItems.Count) - 1; i >= 0; i--) {
                // Each item's transparency increases by 0.25
                Vector2 location = new Vector2(Game1.getOldMouseX() + 16 + 8 * i, Game1.getOldMouseY() + 16 + 8 * i);
                extraData.HeldItems[i].drawInMenu(b, location, 1f, 1f, 0.9f + 0.01f * i, true, new Color(1f, 1f, 1f, 1 - i * 0.25f), true);
            }

            // TODO: Draw a +N if there are more than three stacks of items

            // Draw the upper right close button if it exists
            __instance.upperRightCloseButton?.draw(b);

            // Draw the scroll down button
            if (__instance.downButton != null && ___currentCraftingPage < __instance.pagesOfCraftingRecipes.Count - 1) {
                __instance.downButton.draw(b);
            }

            // Draw the scroll up button
            if (__instance.upButton != null && ___currentCraftingPage > 0) {
                __instance.upButton.draw(b);
            }

            // Draw the mouse if this is the cooking menu
            if (___cooking) {
                __instance.drawMouse(b);
            }

            // Draw the hovered recipe's description
            if (___hoverRecipe != null) {
                // Get the offset
                int xOffset = isHoldingItem ? 48 : 0;
                int yOffset = isHoldingItem ? 48 : 0;

                // Get the buff icons
                string[] buffIconsToDisplay;
                if (___cooking && ___lastCookingHover != null && Game1.objectInformation[___lastCookingHover.ParentSheetIndex].Split('/').Length > 7) {
                    buffIconsToDisplay = Game1.objectInformation[___lastCookingHover.ParentSheetIndex].Split('/')[7].Split(' ');
                } else {
                    buffIconsToDisplay = null;
                }

                // Draw the recipe's description
                IClickableMenu.drawHoverText(b, " ", Game1.smallFont, xOffset, yOffset, -1, ___hoverRecipe.DisplayName, -1, buffIconsToDisplay, ___lastCookingHover, 0, -1, -1, -1, -1, 1f, ___hoverRecipe);
            }

            // Don't use the original drawing code
            return false;

            void DrawHorizontalPartition(int yPosition) {
                b.Draw(Game1.menuTexture, new Vector2(__instance.xPositionOnScreen, yPosition), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 4), Color.White);
                b.Draw(Game1.menuTexture, new Rectangle(__instance.xPositionOnScreen + 64, yPosition, __instance.width - 128, 64), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 6), Color.White);
                b.Draw(Game1.menuTexture, new Vector2(__instance.xPositionOnScreen + __instance.width - 64, yPosition), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 7), Color.White);
            }
        }
        #endregion

        #region CraftingRecipe Patches
        private static bool CraftingRecipe_createItem(CraftingRecipe __instance, ref Item __result) {
            if (!(__instance is ModCraftingRecipe recipe)) {
                return true;
            }

            // throw new InvalidOperationException($"The recipe {recipe.DisplayName} was created by code that should not be called");
            __result = recipe.Recipe.Results.FirstOrDefault() is IItemResult firstResult && firstResult.TryCreateOne(out Item resultItem) ? resultItem : new SObject(Vector2.Zero, Objects.Torch, 1);
            return false;
        }

        private static bool CraftingRecipe_getNumberOfIngredients_Prefix(CraftingRecipe __instance, ref int __result) {
            if (!(__instance is ModCraftingRecipe modRecipe)) {
                return true;
            }

            __result = modRecipe.Recipe.Ingredients.Count();
            return false;
        }

        private static bool CraftingRecipe_drawRecipeDescription_Prefix(CraftingRecipe __instance, SpriteBatch b, Vector2 position, int width, string ___description) {
            if (!(__instance is ModCraftingRecipe modRecipe)) {
                return true;
            }

            // Get all ingredients required for this recipe
            IIngredient[] ingredients = modRecipe.Recipe.Ingredients.ToArray();

            // Create the inventories
            IInventory inventory = new FarmerInventory(Game1.player);
            ChestInventory fridge = new ChestInventory(Utility.getHomeOfFarmer(Game1.player).fridge.Value);

            // Draw horizontal separator
            b.Draw(Game1.staminaRect, new Rectangle((int) (position.X + 8.0), (int) (position.Y + 32.0 + Game1.smallFont.MeasureString("Ing").Y) - 4 - 2, width - 32, 2), Game1.textColor * 0.35f);

            // Draw "Ingredients:"
            Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.567"), Game1.smallFont, position + new Vector2(8f, 28f), Game1.textColor * 0.75f);
            for (int i = 0; i < ingredients.Length; ++i) {
                IIngredient ingredient = ingredients[i];

                // Get ingredient color (red or normal depending on if it's available)
                Color ingredientColor = inventory.Contains(modRecipe.Recipe.Ingredients) ? Game1.textColor : Color.Red;
                if (__instance.isCookingRecipe && fridge.Contains(modRecipe.Recipe.Ingredients)) {
                    ingredientColor = Game1.textColor;
                }

                // Draw the ingredient
                ingredient.Sprite.Draw(b, new Vector2(position.X, position.Y + 64f + i * 64 / 2f + i * 4), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.86f);

                // Draw quantity
                Utility.drawTinyDigits(ingredient.Quantity, b, new Vector2(position.X + 32f - Game1.tinyFont.MeasureString(ingredient.Quantity.ToString()).X, (float) (position.Y + 64.0 + i * 64 / 2f + i * 4 + 21.0)), 2f, 0.87f, Color.AntiqueWhite);

                // Draw ingredient name
                Utility.drawTextWithShadow(b, ingredient.GetDisplayName(), Game1.smallFont, new Vector2((float) (position.X + 32.0 + 8.0), (float) (position.Y + 64.0 + i * 64 / 2f + i * 4 + 4.0)), ingredientColor);
            }

            // Draw horizontal separator
            b.Draw(Game1.staminaRect, new Rectangle((int) position.X + 8, (int) position.Y + 64 + 4 + ingredients.Length * 36, width - 32, 2), Game1.textColor * 0.35f);

            // Draw description
            Utility.drawTextWithShadow(b, Game1.parseText(___description, Game1.smallFont, width - 8), Game1.smallFont, position + new Vector2(0.0f, 76 + ingredients.Length * 36), Game1.textColor * 0.75f);

            // Don't call original method
            return false;
        }
        #endregion

        private class CraftingPageData {
            private readonly CraftingPage _page;

            /// <summary>Items currently held under the cursor.</summary>
            public List<Item> HeldItems { get; } = new List<Item>();

            public CraftingPageData(CraftingPage page) {
                this._page = page;
            }

            public void AddHeldItem(Item addedItem) {
                // Try to stack with an existing held item
                foreach (Item item in this.HeldItems) {
                    // Check if the two items can be combined
                    if (!addedItem.canStackWith(item) || addedItem.Stack + item.Stack >= item.maximumStackSize()) {
                        continue;
                    }

                    // Add the item to the stack
                    item.Stack += addedItem.Stack;
                    return;
                }

                // Add a new stack to the list of held items
                this.HeldItems.Add(addedItem);
            }
        }
    }
}