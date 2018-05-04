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
using TehCore.Weighted;

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
            if (this.MainConfig.ShowFishingData && Game1.player.CurrentTool is CustomFishingRod rod) {
                Color textColor = Color.White;
                SpriteFont font = Game1.smallFont;

                // Draw the fishing GUI to the screen
                SpriteBatch batch = Game1.spriteBatch;
                float boxWidth = 0;
                float lineHeight = font.MeasureString("#").Y;
                float boxHeight = 0;

                // Setup the sprite batch
                using (SpriteBatch hudBatch = new SpriteBatch(Game1.graphics.GraphicsDevice)) {
                    hudBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                    
                    // Draw streak
                    string streakText = ModFishing.Translate("text.streak", FishHelper.GetStreak(Game1.player));
                    hudBatch.DrawStringWithShadow(font, streakText, Vector2.Zero, textColor, 1f);
                    boxWidth = Math.Max(boxWidth, font.MeasureString(streakText).X);
                    boxHeight += lineHeight;

                    // Get info on all the possible fish
                    IWeightedElement<int?>[] possibleFish = FishHelper.GetPossibleFish(Game1.player).ToArray();
                    double totalWeight = possibleFish.SumWeights(); // Should always be 1
                    possibleFish = possibleFish.Where(e => e.Value != null).ToArray();
                    double fishChance = possibleFish.SumWeights() / totalWeight;

                    // Draw treasure chance
                    string treasureText = ModFishing.Translate("text.treasure", ModFishing.Translate("text.percent", FishHelper.GetTreasureChance(Game1.player, rod)));
                    hudBatch.DrawStringWithShadow(font, treasureText, new Vector2(0, boxHeight), textColor, 1f);
                    boxWidth = Math.Max(boxWidth, font.MeasureString(treasureText).X);
                    boxHeight += lineHeight;

                    // Draw trash chance
                    string trashText = ModFishing.Translate("text.trash", ModFishing.Translate("text.percent", 1f - fishChance));
                    hudBatch.DrawStringWithShadow(font, trashText, new Vector2(0, boxHeight), textColor, 1f);
                    boxWidth = Math.Max(boxWidth, font.MeasureString(trashText).X);
                    boxHeight += lineHeight;

                    if (possibleFish.Any()) {
                        // Calculate total weigh of each possible fish (for percentages)
                        totalWeight = possibleFish.SumWeights();

                        // Draw info for each fish
                        const float iconScale = Game1.pixelZoom / 2f;
                        foreach (IWeightedElement<int?> fishData in possibleFish) {
                            // Skip trash
                            if (fishData.Value == null)
                                continue;

                            // Get fish ID
                            int fish = fishData.Value.Value;

                            // Draw fish icon
                            Rectangle source = GameLocation.getSourceRectForObject(fish);
                            hudBatch.Draw(Game1.objectSpriteSheet, new Vector2(0, boxHeight), source, Color.White, 0.0f, Vector2.Zero, iconScale, SpriteEffects.None, 0.8F);
                            lineHeight = Math.Max(lineHeight, source.Height * iconScale);

                            // Draw fish information
                            string chanceText = ModFishing.Translate("text.percent", fishChance * fishData.GetWeight() / totalWeight);
                            string fishText = $"{FishHelper.GetFishName(fish)} - {chanceText}";
                            hudBatch.DrawStringWithShadow(font, fishText, new Vector2(source.Width * iconScale, boxHeight), textColor, 0.8F);
                            boxWidth = Math.Max(boxWidth, font.MeasureString(fishText).X + source.Width * iconScale);

                            // Update destY
                            boxHeight += lineHeight;
                        }
                    }

                    // Draw the background rectangle
                    hudBatch.Draw(ModCore.Instance.WhitePixel, new Rectangle(0, 0, (int) boxWidth, (int) boxHeight), null, new Color(0, 0, 0, 0.25F), 0f, Vector2.Zero, SpriteEffects.None, 0.75F);

                    // Done drawing HUD
                    hudBatch.End();
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
