using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FishingOverhaul.Configs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;
using TehCore;

namespace FishingOverhaul {
    public class ModFishing : Mod {
        public static ModFishing Instance { get; private set; }

        public ConfigMain MainConfig { get; private set; }
        public ConfigFish FishConfig { get; private set; }
        public ConfigTreasure TreasureConfig { get; private set; }

        public override void Entry(IModHelper helper) {
            ModFishing.Instance = this;

            this.LoadConfigs();

            // Make sure this mod is enabled
            if (!this.MainConfig.ModEnabled)
                return;

            GameEvents.UpdateTick += this.UpdateTick;
            GraphicsEvents.OnPostRenderHudEvent += this.PostRenderHud;
            SaveEvents.BeforeSave += this.BeforeSave;
        }

        private void LoadConfigs() {
            // Load configs
            this.MainConfig = this.Helper.TryReadJsonFile<ConfigMain>("config.json") ?? new ConfigMain();
            this.TreasureConfig = this.Helper.TryReadJsonFile<ConfigTreasure>("treasure.json") ?? new ConfigTreasure();
            this.FishConfig = this.Helper.TryReadJsonFile<ConfigFish>("fish.json");

            // Populate fish config if empty
            if (this.FishConfig == null) {
                this.FishConfig = new ConfigFish();
                this.FishConfig.PopulateData();
            }

            // Make sure the configs are generated
            ModCore.Instance.Json.WriteJson("config.json", this.MainConfig, this.Helper, this.MainConfig.MinifyConfigs);
            ModCore.Instance.Json.WriteJson("treasure.json", this.TreasureConfig, this.Helper, this.MainConfig.MinifyConfigs);
            ModCore.Instance.Json.WriteJson("fish.json", this.FishConfig, this.Helper, this.MainConfig.MinifyConfigs);
        }

        #region Events
        private void UpdateTick(object sender, EventArgs e) {
            // Replace the player's fishing rod with a custom rod
            if (Game1.player.CurrentTool is FishingRod rod && !(rod is CustomFishingRod)) {
                Console.WriteLine("Normal rod found, replacing...");
                Game1.player.CurrentTool = new CustomFishingRod(rod);
            }
        }

        private void BeforeSave(object sender, EventArgs eventArgs) {
            for (int i = 0; i < Game1.player.Items.Count; i++) {
                Item item = Game1.player.Items[i];

                if (item is CustomFishingRod rod) {
                    Game1.player.Items[i] = rod.AsNormalFishingRod();
                }
            }
        }

        private void PostRenderHud(object sender, EventArgs eventArgs) {
            if (Game1.player.CurrentTool is CustomFishingRod rod) {
                Color textColor = Color.White;
                SpriteFont font = Game1.smallFont;

                // Draw the fishing GUI to the screen
                SpriteBatch batch = Game1.spriteBatch;
                float lineHeight = font.MeasureString("#").Y;
                float destY = 0;

                // Draw streak
                string streakText = ModFishing.Translate("text.streak", FishHelper.GetStreak(Game1.player));
                batch.DrawStringWithShadow(font, streakText, Vector2.Zero, textColor);
                destY += lineHeight;

                // Get info on all the possible fish
                Dictionary<int, FishData> possibleFish;
                if (Game1.currentLocation is MineShaft m) {
                    possibleFish = FishHelper.GetPossibleFish(m.mineLevel).ToDictionary();
                } else {
                    possibleFish = FishHelper.GetPossibleFish().ToDictionary();
                }

                // Draw treasure chance
                string treasureText = ModFishing.Translate("text.treasure", ModFishing.Translate("text.percent", FishHelper.GetTreasureChance(Game1.player, rod)));
                batch.DrawStringWithShadow(font, treasureText, new Vector2(0, destY), textColor);
                destY += lineHeight;

                // Draw trash chance
                float fishChance = FishHelper.GetFishChance(Game1.player);
                string trashText = ModFishing.Translate("text.trash", ModFishing.Translate("text.percent", 1f - fishChance));
                batch.DrawStringWithShadow(font, trashText, new Vector2(0, destY), textColor );
                destY += lineHeight;

                if (possibleFish.Any()) {
                    // Calculate total weigh of each possible fish (for percentages)
                    double totalWeight = possibleFish.Sum(kv => kv.Value.Chance);
                    
                    // Draw info for each fish
                    const float iconScale = Game1.pixelZoom / 2f;
                    foreach (KeyValuePair<int, FishData> fishData in possibleFish) {
                        // Draw fish icon
                        Rectangle source = GameLocation.getSourceRectForObject(fishData.Key);
                        batch.Draw(Game1.objectSpriteSheet, new Vector2(0, destY), source, Color.White, 0.0f, Vector2.Zero, iconScale, SpriteEffects.None, 1f);
                        lineHeight = Math.Max(lineHeight, source.Height * iconScale);

                        // Draw fish information
                        string chanceText = ModFishing.Translate("text.percent", fishChance * fishData.Value.Chance / totalWeight);
                        batch.DrawStringWithShadow(font, $"{FishHelper.GetFishName(fishData.Key)} - {chanceText}", new Vector2(source.Width * iconScale, destY), textColor);

                        // Update destY
                        destY += lineHeight;
                    }
                }
            }
        }
        #endregion

        #region Static Helpers
        public static string Translate(string key, params object[] formatArgs) {
            return string.Format(ModFishing.Instance.Helper.Translation.Get(key), formatArgs);
        }
        #endregion
    }
}
