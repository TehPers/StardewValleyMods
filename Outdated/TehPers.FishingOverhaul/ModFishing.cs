using System;
using System.Linq;
using System.Text;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;
using StardewValley.Tools;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Json;
using TehPers.CoreMod.Api.Weighted;
using TehPers.FishingOverhaul.Configs;
using TehPers.FishingOverhaul.Json;
using TehPers.FishingOverhaul.Patches;
using SObject = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace TehPers.FishingOverhaul
{
    public class ModFishing : Mod
    {
        public static ModFishing Instance { get; private set; }
        private static readonly Lazy<Texture2D> _whitePixel = new Lazy<Texture2D>(() =>
        {
            var whitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            whitePixel.SetData(new[] { Color.White });
            return whitePixel;
        });

        public FishingApi Api { get; private set; }
        public ConfigMain MainConfig { get; private set; }
        public ConfigFish FishConfig { get; private set; }
        public ConfigFishTraits FishTraitsConfig { get; private set; }
        public ConfigTreasure TreasureConfig { get; private set; }

        internal FishingRodOverrider Overrider { get; set; }
        internal HarmonyInstance Harmony { get; private set; }

        public override void Entry(IModHelper helper)
        {
            ModFishing.Instance = this;

            this.Helper.Events.GameLoop.GameLaunched += (sender, args) =>
            {
                this.Initialize();
            };
        }

        private void Initialize()
        {
            this.Api = new FishingApi();
            var jsonApi = new JsonApi(this);
            //TehMultiplayerApi.GetApi(this).RegisterItem(Objects.Coal, new FishingRodManager());

            // Load the configs
            this.LoadConfigs(jsonApi);

            // Make sure this mod is enabled
            if (!this.MainConfig.ModEnabled)
            {
                return;
            }

            // Apply patches
            this.Harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            this.Harmony.Patch(typeof(NetAudio).GetMethod(nameof(NetAudio.PlayLocal)), prefix: new HarmonyMethod(typeof(NetAudioPatches).GetMethod(nameof(NetAudioPatches.Prefix))));

            // Override fishing
            this.Overrider = new FishingRodOverrider(this);

            // Events
            this.Helper.Events.Display.RenderedHud += (sender, e) => this.PostRenderHud(e);
            this.Helper.Events.Input.ButtonPressed += (sender, e) =>
            {
                if (e.Button != SButton.F5)
                    return;

                this.Monitor.Log("Reloading configs", LogLevel.Info);
                this.LoadConfigs(jsonApi);
                this.Monitor.Log("Done", LogLevel.Trace);
            };

            this.Helper.Events.GameLoop.DayStarted += (sender, e) =>
            {
                this.Monitor.Log("Reloading configs", LogLevel.Info);
                this.LoadConfigs(jsonApi);
                this.Monitor.Log("Done", LogLevel.Trace);
            };

            // Trash data
            this.Api.AddTrashData(new DefaultTrashData());
            this.Api.AddTrashData(new SpecificTrashData(new[] { 152 }, 1, "Beach")); // Seaweed
            this.Api.AddTrashData(new SpecificTrashData(new[] { 153 }, 1, "Farm", invertLocations: true)); // Green Algae
            this.Api.AddTrashData(new SpecificTrashData(new[] { 157 }, 1, "BugLand")); // White Algae
            this.Api.AddTrashData(new SpecificTrashData(new[] { 157 }, 1, "Sewers")); // White Algae
            this.Api.AddTrashData(new SpecificTrashData(new[] { 157 }, 1, "WitchSwamp")); // White Algae
            this.Api.AddTrashData(new SpecificTrashData(new[] { 157 }, 1, "UndergroundMines")); // White Algae
            this.Api.AddTrashData(new SpecificTrashData(new[] { 797 }, 0.01D, "Submarine")); // Pearl
            this.Api.AddTrashData(new SpecificTrashData(new[] { 152 }, 0.99D, "Submarine")); // Seaweed
        }

        public override object GetApi()
        {
            return this.Api;
        }

        private void LoadConfigs(IJsonApi jsonApi)
        {
            // Load configs
            this.MainConfig = jsonApi.ReadOrCreate<ConfigMain>("config.json");
            this.TreasureConfig = jsonApi.ReadOrCreate<ConfigTreasure>("treasure.json", this.MainConfig.MinifyConfigs);

            // Populate fish data
            this.FishConfig = new ConfigFish();
            this.FishConfig.PopulateData();

            // Populate fish traits data
            this.FishTraitsConfig = new ConfigFishTraits();
            this.FishTraitsConfig.PopulateData();

            // Load config values
            FishingRod.maxTackleUses = ModFishing.Instance.MainConfig.DifficultySettings.MaxTackleUses;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TODO: Detatch all event handlers

                this.Overrider.Dispose();
                this.Harmony.UnpatchAll(this.Harmony.Id);
            }

            base.Dispose(disposing);
        }

        #region Events
        private void PostRenderHud(RenderedHudEventArgs args)
        {
            if (!this.MainConfig.ShowFishingData || Game1.eventUp || !(Game1.player.CurrentTool is FishingRod rod))
                return;

            var textColor = Color.White;
            var font = Game1.smallFont;

            // Draw the fishing GUI to the screen
            float boxWidth = 0;
            float lineHeight = font.LineSpacing;
            var boxTopLeft = new Vector2(this.MainConfig.HudTopLeftX, this.MainConfig.HudTopLeftY);
            var boxBottomLeft = boxTopLeft;

            // Setup the sprite batch
            args.SpriteBatch.End();
            args.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            // Draw streak
            var streakText = ModFishing.Translate("text.streak", this.Api.GetStreak(Game1.player));
            args.SpriteBatch.DrawStringWithShadow(font, streakText, boxBottomLeft, textColor, 1f);
            boxWidth = Math.Max(boxWidth, font.MeasureString(streakText).X);
            boxBottomLeft += new Vector2(0, lineHeight);

            // Get info on all the possible fish, grouping all duplicate entries into single entries
            var possibleFish = (from weightedFish in this.Api.GetPossibleFish(Game1.player)
                                where weightedFish.Value != null
                                group weightedFish.GetWeight() by weightedFish.Value.Value into g
                                select new { Weight = g.Sum(), Fish = g.Key })
                .ToWeighted(item => item.Weight, item => item.Fish)
                .ToArray();

            // Calculate the total chance of getting a fish
            var fishChance = possibleFish.SumWeights();

            // Limit the number of displayed fish
            var trimmed = possibleFish.Length - 5;
            if (trimmed > 1)
            {
                possibleFish = possibleFish.Take(5).ToArray();
            }

            // Draw treasure chance
            var treasureText = ModFishing.Translate("text.treasure", ModFishing.Translate("text.percent", this.Api.GetTreasureChance(Game1.player, rod)));
            args.SpriteBatch.DrawStringWithShadow(font, treasureText, boxBottomLeft, textColor, 1f);
            boxWidth = Math.Max(boxWidth, font.MeasureString(treasureText).X);
            boxBottomLeft += new Vector2(0, lineHeight);

            // Draw trash chance
            var trashText = ModFishing.Translate("text.trash", ModFishing.Translate("text.percent", 1f - fishChance));
            args.SpriteBatch.DrawStringWithShadow(font, trashText, boxBottomLeft, textColor, 1f);
            boxWidth = Math.Max(boxWidth, font.MeasureString(trashText).X);
            boxBottomLeft += new Vector2(0, lineHeight);

            if (possibleFish.Any())
            {
                // Draw info for each fish
                const float iconScale = Game1.pixelZoom / 2f;
                foreach (var fishData in possibleFish)
                {
                    // Get fish ID
                    var fish = fishData.Value;

                    // Don't draw hidden fish
                    if (this.Api.IsHidden(fish))
                        continue;

                    // Draw fish icon
                    var source = GameLocation.getSourceRectForObject(fish);
                    args.SpriteBatch.Draw(Game1.objectSpriteSheet, boxBottomLeft, source, Color.White, 0.0f, Vector2.Zero, iconScale, SpriteEffects.None, 1F);
                    lineHeight = Math.Max(lineHeight, source.Height * iconScale);

                    // Draw fish information
                    var chanceText = ModFishing.Translate("text.percent", fishData.GetWeight());
                    var fishText = $"{this.Api.GetFishName(fish)} - {chanceText}";
                    args.SpriteBatch.DrawStringWithShadow(font, fishText, boxBottomLeft + new Vector2(source.Width * iconScale, 0), textColor, 1F);
                    boxWidth = Math.Max(boxWidth, font.MeasureString(fishText).X + source.Width * iconScale);

                    // Update destY
                    boxBottomLeft += new Vector2(0, lineHeight);
                }
            }

            if (trimmed > 0)
            {
                args.SpriteBatch.DrawStringWithShadow(font, $"+{trimmed}...", boxBottomLeft, textColor, 1f);
                boxBottomLeft += new Vector2(0, lineHeight);
            }

            // Draw the background rectangle
            args.SpriteBatch.Draw(_whitePixel.Value, new Rectangle((int)boxTopLeft.X, (int)boxTopLeft.Y, (int)boxWidth, (int)boxBottomLeft.Y), null, new Color(0, 0, 0, 0.25F), 0f, Vector2.Zero, SpriteEffects.None, 0.85F);

            // Debug info
            var text = new StringBuilder();
            if (text.Length > 0)
            {
                args.SpriteBatch.DrawStringWithShadow(Game1.smallFont, text.ToString(), boxBottomLeft, Color.White, 0.8F);
            }

            args.SpriteBatch.End();
            args.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
        }
        #endregion

        #region Static Helpers
        public static string Translate(string key, params object[] formatArgs)
        {
            var translation = ModFishing.Instance.Helper.Translation.Get(key);
            return translation.HasValue() ? string.Format(translation.ToString(), formatArgs) : key;
        }
        #endregion
    }
}
