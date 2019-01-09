using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Environment;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Structs;
using SObject = StardewValley.Object;

namespace TehPers.FestiveSlimes {
    public class ModFestiveSlimes : Mod {
        public static ModFestiveSlimes Instance { get; private set; }
        public static ICoreApi CoreApi;
        private Season _lastSeason;

        public override void Entry(IModHelper helper) {
            ModFestiveSlimes.Instance = this;

            this.Helper.Events.GameLoop.GameLaunched += (sender, args) => {
                if (helper.ModRegistry.GetApi("TehPers.CoreMod") is Func<IMod, ICoreApi> coreFactory) {
                    ModFestiveSlimes.CoreApi = coreFactory(this);
                    this.InitializeMod();
                } else {
                    this.Monitor.Log("Failed to get core API. This mod will be disabled.", LogLevel.Error);
                }
            };
        }

        private void InitializeMod() {
            this.Monitor.Log("Rustling Jimmies...");

            // Replace slimes
            this.ReplaceSlimes();

            // Add custom items
            this.AddItems();

            this.Monitor.Log("Done!");
        }

        private void AddItems() {
            // Add candy to the game
            TextureInformation candyTextureInfo = TextureInformation.FromAssetFile(this.Helper.Content, "assets/candy.png", null, Color.Red);
            BuffDescription candyBuffs = new BuffDescription(TimeSpan.FromMinutes(2.5), speed: 1);
            ModFestiveSlimes.CoreApi.Items.Register("candy", new ModFood(this, "candy", 20, 5, Category.Trash, candyTextureInfo, false, candyBuffs));
        }

        private void ReplaceSlimes() {
            // Add seasonal textures
            // this.Helper.Content.AssetLoaders.Add(new SlimeLoader(this, "Green Slime"));
            // this.Helper.Content.AssetLoaders.Add(new SlimeLoader(this, "Big Slime"));

            // Add seasonal hats
            // this.Helper.Content.AssetEditors.Add(new SlimeHatEditor(this, "Green Slime"));
            // this.Helper.Content.AssetEditors.Add(new SlimeHatEditor(this, "Big Slime"));

            // Make sure to check if the season has changed and invalidate the textures if needed at the start of each day
            // this.Helper.Events.GameLoop.Saved += (sender, args) => this.CheckSeason();
            // this.Helper.Events.GameLoop.SaveLoaded += (sender, args) => this.CheckSeason();

            // Register texture events for green slimes
            ITextureDrawingHelper textureHelper = ModFestiveSlimes.CoreApi.Drawing.GetTextureHelper("Characters/Monsters/Green Slime");
            textureHelper.Drawing += (sender, info) => {
                info.SetTint(Color.White);
            };

            // Register texture events for big slimes
            textureHelper = ModFestiveSlimes.CoreApi.Drawing.GetTextureHelper("Characters/Monsters/Big Slime");
            textureHelper.Drawing += (sender, info) => {
                info.SetTint(Color.White);
            };

            // Add additional drops to slimes
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // Green slime
            MethodInfo target = typeof(GreenSlime).GetMethod(nameof(GreenSlime.getExtraDropItems), BindingFlags.Public | BindingFlags.Instance);
            MethodInfo replacement = typeof(ModFestiveSlimes).GetMethod(nameof(ModFestiveSlimes.GreenSlime_GetExtraDropItemsPostfix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, postfix: new HarmonyMethod(replacement));

            // Big slime
            target = typeof(Monster).GetMethod(nameof(Monster.getExtraDropItems), BindingFlags.Public | BindingFlags.Instance);
            replacement = typeof(ModFestiveSlimes).GetMethod(nameof(ModFestiveSlimes.Monster_GetExtraDropItemsPostfix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, postfix: new HarmonyMethod(replacement));

            // TODO: Debug
            this.Helper.Events.Input.ButtonReleased += (sender, args) => {
                if (args.Button == SButton.OemSemicolon) {
                    Game1.currentLocation.addCharacter(new GreenSlime(Game1.player.Position, Color.Red));
                }
                if (args.Button == SButton.L) {
                    Game1.currentLocation.addCharacter(new BigSlime(Game1.player.Position, 0));
                }
            };

            this.Helper.Events.Display.RenderedWorld += (sender, args) => {
                if (Game1.currentLocation != null) {
                    foreach (NPC character in Game1.currentLocation?.characters) {
                        character.draw(args.SpriteBatch);
                    }
                }
            };
        }

        private void CheckSeason() {
            // Check if the season changed
            Season season = SDateTime.Today.Season;
            if (this._lastSeason != season) {
                this.Helper.Content.InvalidateCache("Characters/Monsters/Green Slime");
                this.Helper.Content.InvalidateCache("Characters/Monsters/Big Slime");
            }

            // Update the tracked season
            this._lastSeason = season;
        }

        // List<Item> GreenSlime.getExtraDropItems()
        private static void GreenSlime_GetExtraDropItemsPostfix(ref List<Item> __result) {
            if (SDateTime.Today.Season == Season.Fall && Game1.random.NextDouble() < 0.25 && ModFestiveSlimes.CoreApi.Items.TryGetInformation("candy", out IObjectInformation candyInfo) && candyInfo.Index is int index) {
                __result.Add(new SObject(Vector2.Zero, index, 1));
            }
        }

        // List<Item> BigSlime.getExtraDropItems()
        private static void Monster_GetExtraDropItemsPostfix(Monster __instance, ref List<Item> __result) {
            if (!(__instance is BigSlime)) {
                return;
            }

            if (SDateTime.Today.Season == Season.Fall && ModFestiveSlimes.CoreApi.Items.TryGetInformation("candy", out IObjectInformation candyInfo) && candyInfo.Index is int index) {
                if (Game1.random.NextDouble() < 0.5) {
                    __result.Add(new SObject(Vector2.Zero, index, 1));
                }

                if (Game1.random.NextDouble() < 0.25) {
                    __result.Add(new SObject(Vector2.Zero, index, 1));
                }
            }
        }
    }
}
