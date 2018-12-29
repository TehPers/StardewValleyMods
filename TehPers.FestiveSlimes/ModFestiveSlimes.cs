using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Items;
using BuffDescription = TehPers.CoreMod.Api.Items.BuffDescription;
using Category = TehPers.CoreMod.Api.Items.Category;
using SObject = StardewValley.Object;
using TextureInformation = TehPers.CoreMod.Api.Drawing.TextureInformation;

namespace TehPers.FestiveSlimes {
    public class ModFestiveSlimes : Mod {
        public static ModFestiveSlimes Instance { get; private set; }
        public MonsterAssetLoader MonsterAssets { get; private set; }
        private static ICoreApi _coreApi;

        public override void Entry(IModHelper helper) {
            ModFestiveSlimes.Instance = this;

            GameEvents.FirstUpdateTick += (sender, args) => {
                if (helper.ModRegistry.GetApi("TehPers.CoreMod") is Func<IMod, ICoreApi> coreFactory) {
                    ModFestiveSlimes._coreApi = coreFactory(this);
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
            BuffDescription candyBuffs = new BuffDescription(TimeSpan.FromMinutes(2.5)) {
                Speed = 1
            };
            ModFestiveSlimes._coreApi.Items.Register("candy", new ModFood(this, "candy", 20, 5, Category.Trash, candyTextureInfo, false, candyBuffs));
        }

        private void ReplaceSlimes() {
            // Add asset loader for custom slime textures
            this.MonsterAssets = new MonsterAssetLoader(this);
            this.Helper.Content.AssetLoaders.Add(this.MonsterAssets);
            Texture2D greenSlime = null;
            Texture2D bigSlime = null;

            // Make sure to check if the season has changed and invalidate the textures if needed
            SaveEvents.AfterSave += (sender, args) => this.CheckSeason(ref greenSlime, ref bigSlime);
            SaveEvents.AfterLoad += (sender, args) => this.CheckSeason(ref greenSlime, ref bigSlime);

            // Prevent slimes from being tinted when the texture is replaced
            ModFestiveSlimes._coreApi.Drawing.AddOverride(info => {
                if (!this.MonsterAssets.Replaced) {
                    return;
                }

                if (info.Texture == greenSlime || info.Texture == bigSlime) {
                    info.SetTint(Color.White);
                }
            });

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
        }

        private void CheckSeason(ref Texture2D greenSlime, ref Texture2D bigSlime) {
            this.MonsterAssets.CheckForChanges();
            greenSlime = new GreenSlime(Vector2.Zero).Sprite.Texture;
            bigSlime = new BigSlime(Vector2.Zero, (Game1.currentLocation as MineShaft)?.mineLevel ?? 0).Sprite.Texture;
        }

        // List<Item> GreenSlime.getExtraDropItems()
        private static void GreenSlime_GetExtraDropItemsPostfix(ref List<Item> __result) {
            if (!ModFestiveSlimes.Instance.MonsterAssets.Replaced) {
                return;
            }

            if (Game1.random.NextDouble() < 0.25 && ModFestiveSlimes._coreApi.Items.TryGetInformation("candy", out IObjectInformation candyInfo) && candyInfo.Index is int index) {
                __result.Add(new SObject(Vector2.Zero, index, 1));
            }
        }

        // List<Item> BigSlime.getExtraDropItems()
        private static void Monster_GetExtraDropItemsPostfix(Monster __instance, ref List<Item> __result) {
            if (!(__instance is BigSlime)) {
                return;
            }

            if (!ModFestiveSlimes.Instance.MonsterAssets.Replaced) {
                return;
            }

            if (ModFestiveSlimes._coreApi.Items.TryGetInformation("candy", out IObjectInformation candyInfo) && candyInfo.Index is int index) {
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
