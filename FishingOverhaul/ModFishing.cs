using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using TehPers.Core;
using TehPers.Core.Api.Enums;
using TehPers.Core.Api.Weighted;
using TehPers.Core.Helpers;
using TehPers.FishingOverhaul.Api.Enums;
using TehPers.FishingOverhaul.Configs;

namespace TehPers.FishingOverhaul {
    public class ModFishing : Mod {
        public static ModFishing Instance { get; private set; }

        public FishingApi Api { get; }
        public ConfigMain MainConfig { get; private set; }
        public ConfigFish FishConfig { get; private set; }
        public ConfigFishTraits FishTraitsConfig { get; private set; }
        public ConfigTreasure TreasureConfig { get; private set; }

        internal FishingRodOverrider Overrider { get; set; }
        internal HarmonyInstance Harmony { get; private set; }

        public ModFishing() {
            this.Api = new FishingApi();
        }

        public override void Entry(IModHelper helper) {
            ModFishing.Instance = this;

            this.LoadConfigs();

            // Make sure this mod is enabled
            if (!this.MainConfig.ModEnabled)
                return;

            // Apply patches
            this.Harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            this.Harmony.PatchAll(Assembly.GetExecutingAssembly());

            // Easter eggs I guess
            this.Api.SetFishTraits(Objects.Diamond, new FishTraits {
                Difficulty = 100,
                MinSize = 1,
                MaxSize = 3,
                MotionType = FishMotionType.DART
            });

            this.Api.SetFishData("Town", Objects.Diamond, new FishData(0.1, new FishData.TimeInterval[] { new FishData.TimeInterval(600, 2600) }, WaterType.Both, Season.Spring | Season.Summer | Season.Fall | Season.Winter));
            this.Api.HideFish(Objects.Diamond);

            this.Overrider = new FishingRodOverrider();
            GraphicsEvents.OnPostRenderHudEvent += this.PostRenderHud;
            /*ControlEvents.KeyPressed += (sender, pressed) => {
                if (pressed.KeyPressed == Keys.NumPad7) {
                    Menu menu = new Menu(Game1.viewport.Width / 6, Game1.viewport.Height / 6, 2 * Game1.viewport.Width / 3, 2 * Game1.viewport.Height / 3);
                    menu.MainElement.AddChild(new TextElement {
                        Text = "Test Menu",
                        Color = Color.Black,
                        Size = new BoxVector(0, 50, 1F, 0F),
                        Scale = new Vector2(3, 3),
                        HorizontalAlignment = Alignment.MIDDLE,
                        VerticalAlignment = Alignment.TOP
                    });
                    menu.MainElement.AddChild(new TextboxElement {
                        Location = new BoxVector(0, 100, 0, 0)
                    });
                    ModCore.Instance.ShowMenu(menu);
                }
            };*/
        }

        public override object GetApi() {
            return this.Api;
        }

        private void LoadConfigs() {
            // Load configs
            this.MainConfig = ModCore.Instance.JsonHelper.ReadOrCreate<ConfigMain>("config.json", this.Helper);
            this.TreasureConfig = ModCore.Instance.JsonHelper.ReadOrCreate<ConfigTreasure>("treasure.json", this.Helper, this.MainConfig.MinifyConfigs);
            this.FishConfig = ModCore.Instance.JsonHelper.ReadOrCreate("fish.json", this.Helper, () => {
                // Populate fish data
                ConfigFish config = new ConfigFish();
                config.PopulateData();
                return config;
            }, this.MainConfig.MinifyConfigs);
            this.FishTraitsConfig = ModCore.Instance.JsonHelper.ReadOrCreate("fishTraits.json", this.Helper, () => {
                // Populate fish traits data
                ConfigFishTraits config = new ConfigFishTraits();
                config.PopulateData();
                return config;
            }, this.MainConfig.MinifyConfigs);

            // Not a config, but whatever
            this.Api.AddTrashData(new DefaultTrashData());
            this.Api.AddTrashData(new SpecificTrashData(new[] { 797 }, 0.01D, "Submarine")); // Pearl
            this.Api.AddTrashData(new SpecificTrashData(new[] { 152 }, 0.99D, "Submarine")); // Seaweed
        }

        #region Events
        private void PostRenderHud(object sender, EventArgs eventArgs) {
            if (!this.MainConfig.ShowFishingData || Game1.eventUp || !(Game1.player.CurrentTool is FishingRod rod))
                return;

            Color textColor = Color.White;
            SpriteFont font = Game1.smallFont;

            // Draw the fishing GUI to the screen
            float boxWidth = 0;
            float lineHeight = font.LineSpacing;
            float boxHeight = 0;

            // Setup the sprite batch
            using (SpriteBatch batch = new SpriteBatch(Game1.graphics.GraphicsDevice)) {
                batch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

                // Draw streak
                string streakText = ModFishing.Translate("text.streak", this.Api.GetStreak(Game1.player));
                batch.DrawStringWithShadow(font, streakText, Vector2.Zero, textColor, 1f);
                boxWidth = Math.Max(boxWidth, font.MeasureString(streakText).X);
                boxHeight += lineHeight;

                // Get info on all the possible fish
                IWeightedElement<int?>[] possibleFish = this.Api.GetPossibleFish(Game1.player).ToArray();
                double totalWeight = possibleFish.SumWeights(); // Should always be 1
                possibleFish = possibleFish.Where(e => e.Value != null).ToArray();
                double fishChance = possibleFish.SumWeights() / totalWeight;

                // Draw treasure chance
                string treasureText = ModFishing.Translate("text.treasure", ModFishing.Translate("text.percent", this.Api.GetTreasureChance(Game1.player, rod)));
                batch.DrawStringWithShadow(font, treasureText, new Vector2(0, boxHeight), textColor, 1f);
                boxWidth = Math.Max(boxWidth, font.MeasureString(treasureText).X);
                boxHeight += lineHeight;

                // Draw trash chance
                string trashText = ModFishing.Translate("text.trash", ModFishing.Translate("text.percent", 1f - fishChance));
                batch.DrawStringWithShadow(font, trashText, new Vector2(0, boxHeight), textColor, 1f);
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

                        // Don't draw hidden fish
                        if (this.Api.IsHidden(fish))
                            continue;

                        // Draw fish icon
                        Rectangle source = GameLocation.getSourceRectForObject(fish);
                        batch.Draw(Game1.objectSpriteSheet, new Vector2(0, boxHeight), source, Color.White, 0.0f, Vector2.Zero, iconScale, SpriteEffects.None, 0.8F);
                        lineHeight = Math.Max(lineHeight, source.Height * iconScale);

                        // Draw fish information
                        string chanceText = ModFishing.Translate("text.percent", fishChance * fishData.GetWeight() / totalWeight);
                        string fishText = $"{this.Api.GetFishName(fish)} - {chanceText}";
                        batch.DrawStringWithShadow(font, fishText, new Vector2(source.Width * iconScale, boxHeight), textColor, 0.8F);
                        boxWidth = Math.Max(boxWidth, font.MeasureString(fishText).X + source.Width * iconScale);

                        // Update destY
                        boxHeight += lineHeight;
                    }
                }

                // Draw the background rectangle
                batch.Draw(ModCore.Instance.WhitePixel, new Rectangle(0, 0, (int) boxWidth, (int) boxHeight), null, new Color(0, 0, 0, 0.25F), 0f, Vector2.Zero, SpriteEffects.None, 0.75F);

                // Debug info
                //Point[] floodedTiles = Game1.currentLocation?.GetFloodedTiles(10).ToArray() ?? new Point[0];
                StringBuilder text = new StringBuilder();
                //text.AppendLine($"Hover Key: {Enum.GetName(typeof(Keys), ModCore.Instance.InputHelper._heldKey ?? Keys.None)}");
                //text.AppendLine($"Time Pressed: {DateTime.UtcNow - ModCore.Instance.InputHelper._keyStart:g}");
                //text.AppendLine($"Time Since Repeat: {DateTime.UtcNow - ModCore.Instance.InputHelper._lastRepeat:g}");
                //text.AppendLine($"Flooded tile? {floodedTiles.Contains(new Point(Game1.player.getTileX(), Game1.player.getTileY()))}");
                batch.DrawStringWithShadow(Game1.smallFont, text.ToString(), new Vector2(0, boxHeight), Color.White, 0.8F);

                //foreach (Point floodedTile in floodedTiles) {
                //    Rectangle rect = new Rectangle(floodedTile.X * Game1.tileSize - Game1.viewport.X, floodedTile.Y * Game1.tileSize - Game1.viewport.Y, Game1.tileSize, Game1.tileSize);
                //    batch.FillRectangle(rect, new Color(0, 1F, 0F, 0.25F));
                //}

                // Done drawing HUD
                batch.End();
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
