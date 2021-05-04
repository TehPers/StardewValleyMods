using System;
using System.Linq;
using System.Text;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.Tools;
using TehPers.Core.Helpers.Static;
using TehPers.Core.Json;
using TehPers.Core.Rewrite;
using TehPers.FishingOverhaul.Configs;
using TehPers.FishingOverhaul.Patches;
using SObject = StardewValley.Object;

namespace TehPers.FishingOverhaul {
    public class ModFishing : Mod {
        public static ModFishing Instance { get; private set; }

        public FishingApi Api { get; private set; }
        public ConfigMain MainConfig { get; private set; }
        public ConfigFish FishConfig { get; private set; }
        public ConfigFishTraits FishTraitsConfig { get; private set; }
        public ConfigTreasure TreasureConfig { get; private set; }

        internal FishingRodOverrider Overrider { get; set; }
        internal HarmonyInstance Harmony { get; private set; }

        public override void Entry(IModHelper helper) {
            Instance = this;
            Api = new FishingApi();
            //TehMultiplayerApi.GetApi(this).RegisterItem(Objects.Coal, new FishingRodManager());

            // Make sure TehPers.Core isn't loaded as it's not needed anymore
            if (helper.ModRegistry.IsLoaded("TehPers.Core")) {
                Monitor.Log("Delete TehCore, it's not needed anymore. Your game will probably crash with it installed anyway.", LogLevel.Error);
            }

            // Load the configs
            LoadConfigs();

            // Make sure this mod is enabled
            if (!MainConfig.ModEnabled) {
                return;
            }

            // Apply patches
            Harmony = HarmonyInstance.Create(ModManifest.UniqueID);
            var targetType = AssortedHelpers.GetSDVType(nameof(NetAudio));
            Harmony.Patch(targetType.GetMethod(nameof(NetAudio.PlayLocal)), new HarmonyMethod(typeof(NetAudioPatches).GetMethod(nameof(NetAudioPatches.Prefix))));

            // Override fishing
            Overrider = new FishingRodOverrider(this);

            // Events
            Helper.Events.Display.RenderedHud += PostRenderHud;
            Helper.Events.Input.ButtonPressed += (sender, e) => {
                if (e.Button != SButton.F5)
                    return;

                Monitor.Log("Reloading configs...", LogLevel.Info);
                LoadConfigs();
                Monitor.Log("Done", LogLevel.Info);
            };

            // Trash data
            Api.AddTrashData(new DefaultTrashData());
            Api.AddTrashData(new SpecificTrashData(new[] { 152 }, 1, "Beach")); // Seaweed
            Api.AddTrashData(new SpecificTrashData(new[] { 153 }, 1, "Farm", invertLocations: true)); // Green Algae
            Api.AddTrashData(new SpecificTrashData(new[] { 157 }, 1, "BugLand")); // White Algae
            Api.AddTrashData(new SpecificTrashData(new[] { 157 }, 1, "Sewers")); // White Algae
            Api.AddTrashData(new SpecificTrashData(new[] { 157 }, 1, "WitchSwamp")); // White Algae
            Api.AddTrashData(new SpecificTrashData(new[] { 157 }, 1, "UndergroundMines")); // White Algae
            Api.AddTrashData(new SpecificTrashData(new[] { 797 }, 0.01D, "Submarine")); // Pearl
            Api.AddTrashData(new SpecificTrashData(new[] { 152 }, 0.99D, "Submarine")); // Seaweed

            // this.LoadGuis();

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

                    Game1.activeClickableMenu = menu;
                }
            };*/
        }

        public override object GetApi() {
            return Api;
        }

        private void LoadConfigs() {
            var jsonApi = this.GetCoreApi().GetJsonApi();

            // Load configs
            MainConfig = jsonApi.ReadOrCreate<ConfigMain>("config.json");
            TreasureConfig = jsonApi.ReadOrCreate<ConfigTreasure>("treasure.json", MainConfig.MinifyConfigs);
            FishConfig = jsonApi.ReadOrCreate("fish.json", () => {
                // Populate fish data
                var config = new ConfigFish();
                config.PopulateData();
                return config;
            }, MainConfig.MinifyConfigs);
            FishTraitsConfig = jsonApi.ReadOrCreate("fishTraits.json", () => {
                // Populate fish traits data
                var config = new ConfigFishTraits();
                config.PopulateData();
                return config;
            }, MainConfig.MinifyConfigs);

            // Load config values
            FishingRod.maxTackleUses = Instance.MainConfig.DifficultySettings.MaxTackleUses;
        }

        #region Events
        private void PostRenderHud(object sender, EventArgs eventArgs) {
            if (!MainConfig.ShowFishingData || Game1.eventUp || !(Game1.player.CurrentTool is FishingRod rod))
                return;

            var textColor = Color.White;
            var font = Game1.smallFont;

            // Draw the fishing GUI to the screen
            float boxWidth = 0;
            float lineHeight = font.LineSpacing;
            var boxTopLeft = new Vector2(MainConfig.HudTopLeftX, MainConfig.HudTopLeftY);
            var boxBottomLeft = boxTopLeft;

            // Setup the sprite batch
            var batch = Game1.spriteBatch;
            batch.End();
            batch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            // Draw streak
            var streakText = Translate("text.streak", Api.GetStreak(Game1.player));
            batch.DrawStringWithShadow(font, streakText, boxBottomLeft, textColor, 1f);
            boxWidth = Math.Max(boxWidth, font.MeasureString(streakText).X);
            boxBottomLeft += new Vector2(0, lineHeight);

            // Get info on all the possible fish, grouping all duplicate entries into single entries
            var possibleFish = (from weightedFish in Api.GetPossibleFish(Game1.player)
                                                    where weightedFish.Value != null
                                                    group weightedFish.GetWeight() by weightedFish.Value.Value into g
                                                    select new { Weight = g.Sum(), Fish = g.Key })
                .ToWeighted(item => item.Weight, item => item.Fish)
                .ToArray();

            // Calculate the total chance of getting a fish
            var fishChance = possibleFish.SumWeights();

            // Limit the number of displayed fish
            var trimmed = 0;
            if (MainConfig.HudMaxFishTypes > 0) {
                trimmed = possibleFish.Length - MainConfig.HudMaxFishTypes;
                if (trimmed > 0) {
                    possibleFish = possibleFish.Take(MainConfig.HudMaxFishTypes).ToArray();
                }
            }

            // Draw treasure chance
            var treasureText = Translate("text.treasure", Translate("text.percent", Api.GetTreasureChance(Game1.player, rod)));
            batch.DrawStringWithShadow(font, treasureText, boxBottomLeft, textColor, 1f);
            boxWidth = Math.Max(boxWidth, font.MeasureString(treasureText).X);
            boxBottomLeft += new Vector2(0, lineHeight);

            // Draw trash chance
            var trashText = Translate("text.trash", Translate("text.percent", 1f - fishChance));
            batch.DrawStringWithShadow(font, trashText, boxBottomLeft, textColor, 1f);
            boxWidth = Math.Max(boxWidth, font.MeasureString(trashText).X);
            boxBottomLeft += new Vector2(0, lineHeight);

            if (possibleFish.Any()) {
                // Draw info for each fish
                const float iconScale = Game1.pixelZoom / 2f;
                foreach (var fishData in possibleFish) {
                    // Get fish ID
                    var fish = fishData.Value;

                    // Don't draw hidden fish
                    if (Api.IsHidden(fish))
                        continue;

                    // Draw fish icon
                    var source = GameLocation.getSourceRectForObject(fish);
                    batch.Draw(Game1.objectSpriteSheet, boxBottomLeft, source, Color.White, 0.0f, Vector2.Zero, iconScale, SpriteEffects.None, 1F);
                    lineHeight = Math.Max(lineHeight, source.Height * iconScale);

                    // Draw fish information
                    var chanceText = Translate("text.percent", fishData.GetWeight());
                    var fishText = $"{Api.GetFishName(fish)} - {chanceText}";
                    batch.DrawStringWithShadow(font, fishText, boxBottomLeft + new Vector2(source.Width * iconScale, 0), textColor, 1F);
                    boxWidth = Math.Max(boxWidth, font.MeasureString(fishText).X + source.Width * iconScale);

                    // Update destY
                    boxBottomLeft += new Vector2(0, lineHeight);
                }
            }

            if (trimmed > 0) {
                batch.DrawStringWithShadow(font, $"+{trimmed}...", boxBottomLeft, textColor, 1f);
                boxBottomLeft += new Vector2(0, lineHeight);
            }

            // Draw the background rectangle
            batch.Draw(DrawHelpers.WhitePixel, new Rectangle((int) boxTopLeft.X, (int) boxTopLeft.Y, (int) boxWidth, (int) boxBottomLeft.Y), null, new Color(0, 0, 0, 0.25F), 0f, Vector2.Zero, SpriteEffects.None, 0.85F);

            // Debug info
            var text = new StringBuilder();
            if (text.Length > 0) {
                batch.DrawStringWithShadow(Game1.smallFont, text.ToString(), boxBottomLeft, Color.White, 0.8F);
            }

            batch.End();
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
        }
        #endregion

        #region Static Helpers
        public static string Translate(string key, params object[] formatArgs) {
            var translation = Instance.Helper.Translation.Get(key);
            return translation.HasValue() ? string.Format(translation.ToString(), formatArgs) : key;
        }
        #endregion
    }
}
