using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Tools;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Configs;

namespace TehPers.FishingOverhaul {
    public class CustomBobberBar : BobberBar {
        private readonly IReflectedField<bool> _treasure;
        private readonly IReflectedField<bool> _treasureCaught;
        private readonly IReflectedField<float> _treasurePosition;
        private readonly IReflectedField<float> _treasureAppearTimer;
        private readonly IReflectedField<float> _treasureScale;

        private readonly IReflectedField<float> _distanceFromCatching;
        private readonly IReflectedField<float> _treasureCatchLevel;

        private readonly IReflectedField<float> _bobberBarPos;
        private readonly IReflectedField<bool> _bobberInBar;
        private readonly IReflectedField<float> _difficulty;
        private readonly IReflectedField<int> _fishQuality;
        private readonly IReflectedField<bool> _perfect;
        private readonly IReflectedField<float> _scale;
        private readonly IReflectedField<bool> _flipBubble;
        private readonly IReflectedField<int> _bobberBarHeight;
        private readonly IReflectedField<float> _reelRotation;
        private readonly IReflectedField<float> _bobberPosition;
        private readonly IReflectedField<bool> _bossFish;
        private readonly IReflectedField<int> _motionType;
        private readonly IReflectedField<int> _fishSize;
        private readonly IReflectedField<int> _whichFish;

        private readonly IReflectedField<Vector2> _barShake;
        private readonly IReflectedField<Vector2> _fishShake;
        private readonly IReflectedField<Vector2> _treasureShake;
        private readonly IReflectedField<Vector2> _everythingShake;

        private readonly IReflectedField<SparklingText> _sparkleText;

        private float _lastDistanceFromCatching;
        private float _lastTreasureCatchLevel;
        private bool _perfectChanged;
        private bool _treasureChanged;
        private bool _notifiedFailOrSucceed;
        private readonly int _origStreak;
        private readonly int _origQuality;
        private readonly int _origFish;

        public Farmer User { get; }
        public bool Unaware { get; }

        public CustomBobberBar(IMod mod, Farmer user, int whichFish, float fishSize, bool treasure, int bobber) : base(whichFish, fishSize, treasure, bobber) {
            this.User = user;
            this._origStreak = ModFishing.Instance.Api.GetStreak(user);
            this._origFish = whichFish;

            /* Private field hooks */
            this._treasure = mod.Helper.Reflection.GetField<bool>(this, "treasure");
            this._treasure = mod.Helper.Reflection.GetField<bool>(this, "treasure");
            this._treasureCaught = mod.Helper.Reflection.GetField<bool>(this, "treasureCaught");
            this._treasurePosition = mod.Helper.Reflection.GetField<float>(this, "treasurePosition");
            this._treasureAppearTimer = mod.Helper.Reflection.GetField<float>(this, "treasureAppearTimer");
            this._treasureScale = mod.Helper.Reflection.GetField<float>(this, "treasureScale");

            this._distanceFromCatching = mod.Helper.Reflection.GetField<float>(this, "distanceFromCatching");
            this._treasureCatchLevel = mod.Helper.Reflection.GetField<float>(this, "treasureCatchLevel");

            this._bobberBarPos = mod.Helper.Reflection.GetField<float>(this, "bobberBarPos");
            this._bobberInBar = mod.Helper.Reflection.GetField<bool>(this, "bobberInBar");
            this._difficulty = mod.Helper.Reflection.GetField<float>(this, "difficulty");
            this._fishQuality = mod.Helper.Reflection.GetField<int>(this, "fishQuality");
            this._perfect = mod.Helper.Reflection.GetField<bool>(this, "perfect");
            this._scale = mod.Helper.Reflection.GetField<float>(this, "scale");
            this._flipBubble = mod.Helper.Reflection.GetField<bool>(this, "flipBubble");
            this._bobberBarHeight = mod.Helper.Reflection.GetField<int>(this, "bobberBarHeight");
            this._reelRotation = mod.Helper.Reflection.GetField<float>(this, "reelRotation");
            this._bobberPosition = mod.Helper.Reflection.GetField<float>(this, "bobberPosition");
            this._bossFish = mod.Helper.Reflection.GetField<bool>(this, "bossFish");
            this._motionType = mod.Helper.Reflection.GetField<int>(this, "motionType");
            this._fishSize = mod.Helper.Reflection.GetField<int>(this, "fishSize");
            this._whichFish = mod.Helper.Reflection.GetField<int>(this, "whichFish");

            this._barShake = mod.Helper.Reflection.GetField<Vector2>(this, "barShake");
            this._fishShake = mod.Helper.Reflection.GetField<Vector2>(this, "fishShake");
            this._treasureShake = mod.Helper.Reflection.GetField<Vector2>(this, "treasureShake");
            this._everythingShake = mod.Helper.Reflection.GetField<Vector2>(this, "everythingShake");

            this._sparkleText = mod.Helper.Reflection.GetField<SparklingText>(this, "sparkleText");

            this._lastDistanceFromCatching = this._distanceFromCatching.GetValue();
            this._lastTreasureCatchLevel = this._treasureCatchLevel.GetValue();

            /* Actual code */
            ConfigMain config = ModFishing.Instance.MainConfig;
            IFishTraits traits = ModFishing.Instance.Api.GetFishTraits(whichFish);

            // Check if fish is unaware
            this.Unaware = Game1.random.NextDouble() < ModFishing.Instance.Api.GetUnawareChance(user, whichFish);

            // Applies difficulty modifier, including if fish is unaware
            float difficulty = traits?.Difficulty ?? this._difficulty.GetValue();
            difficulty *= config.DifficultySettings.BaseDifficultyMult;
            difficulty *= 1F + this._origStreak * config.DifficultySettings.DifficultyStreakEffect;
            if (this.Unaware) {
                difficulty *= config.UnawareSettings.UnawareMult;
                Game1.showGlobalMessage(ModFishing.Translate("text.unaware", ModFishing.Translate("text.percent", 1F - config.UnawareSettings.UnawareMult)));
            }
            this._difficulty.SetValue(difficulty);

            // Adjusts additional traits about the fish
            if (traits != null) {
                this._motionType.SetValue((int) traits.MotionType);
                this._fishSize.SetValue(traits.MinSize + (int) ((traits.MaxSize - traits.MinSize) * fishSize) + 1);
            }

            // Adjusts quality to be increased by streak
            int fishQuality = this._fishQuality.GetValue();
            this._origQuality = fishQuality;
            int qualityBonus = (int) Math.Floor((double) this._origStreak / config.StreakSettings.StreakForIncreasedQuality);
            fishQuality = Math.Min(fishQuality + qualityBonus, 3);
            if (fishQuality == 3) fishQuality++; // Iridium-quality fish. Only possible through your perfect streak
            this._fishQuality.SetValue(fishQuality);

            // Increase the user's perfect streak (this will be dropped to 0 if they don't get a perfect catch)
            if (this._origStreak >= config.StreakSettings.StreakForIncreasedQuality)
                this._sparkleText.SetValue(new SparklingText(Game1.dialogueFont, ModFishing.Translate("text.streak", this._origStreak), Color.Yellow, Color.White));
        }

        public override void update(GameTime time) {
            // Speed warp on catching fish
            float distanceFromCatching = this._distanceFromCatching.GetValue();
            float delta = distanceFromCatching - this._lastDistanceFromCatching;
            float mult = delta > 0 ? ModFishing.Instance.MainConfig.DifficultySettings.CatchSpeed : ModFishing.Instance.MainConfig.DifficultySettings.DrainSpeed;
            distanceFromCatching = this._lastDistanceFromCatching + delta * mult;
            this._lastDistanceFromCatching = distanceFromCatching;
            this._distanceFromCatching.SetValue(distanceFromCatching);

            // Speed warp on catching treasure
            float treasureCatchLevel = this._treasureCatchLevel.GetValue();
            delta = treasureCatchLevel - this._lastTreasureCatchLevel;
            mult = delta > 0 ? ModFishing.Instance.MainConfig.DifficultySettings.TreasureCatchSpeed : ModFishing.Instance.MainConfig.DifficultySettings.TreasureDrainSpeed;
            treasureCatchLevel = this._lastTreasureCatchLevel + delta * mult;
            this._lastTreasureCatchLevel = treasureCatchLevel;
            this._treasureCatchLevel.SetValue(treasureCatchLevel);

            bool perfect = this._perfect.GetValue();
            bool treasure = this._treasure.GetValue();
            bool treasureCaught = this._treasureCaught.GetValue();

            // Check if still perfect, otherwise apply changes to loot
            if (!this._perfectChanged && !perfect) {
                this._perfectChanged = true;
                this._fishQuality.SetValue(Math.Min(this._origQuality, ModFishing.Instance.MainConfig.DifficultySettings.PreventGoldOnNormalCatch ? 1 : 2));
                ModFishing.Instance.Api.SetStreak(this.User, 0);
                if (this._origStreak >= ModFishing.Instance.MainConfig.StreakSettings.StreakForIncreasedQuality) {
                    Game1.showGlobalMessage(ModFishing.Translate(treasure ? "text.warnStreak" : "text.lostStreak", this._origStreak));
                }
            }

            // Check if lost perfect, but got treasure
            if (!this._treasureChanged && !perfect && treasure && treasureCaught) {
                this._treasureChanged = true;
                int qualityBonus = (int) Math.Floor((double) this._origStreak / ModFishing.Instance.MainConfig.StreakSettings.StreakForIncreasedQuality);
                int quality = this._origQuality;
                quality = Math.Min(quality + qualityBonus, 3);
                if (quality == 3) quality++;
                this._fishQuality.SetValue(quality);
            }

            // Base call
            base.update(time);

            // Check if done fishing
            distanceFromCatching = this._distanceFromCatching.GetValue();
            if (this._notifiedFailOrSucceed)
                return;

            if (distanceFromCatching <= 0.0) {
                // Failed to catch fish
                this._notifiedFailOrSucceed = true;

                if (treasure) {
                    this._notifiedFailOrSucceed = true;
                    if (this._origStreak >= ModFishing.Instance.MainConfig.StreakSettings.StreakForIncreasedQuality) {
                        Game1.showGlobalMessage(ModFishing.Translate("text.lostStreak", this._origStreak));
                    }
                }
            } else if (distanceFromCatching >= 1.0) {
                // Succeeded in catching the fish
                this._notifiedFailOrSucceed = true;

                if (perfect) {
                    ModFishing.Instance.Api.SetStreak(this.User, this._origStreak + 1);
                } else if (treasure && treasureCaught) {
                    if (this._origStreak >= ModFishing.Instance.MainConfig.StreakSettings.StreakForIncreasedQuality)
                        Game1.showGlobalMessage(ModFishing.Translate("text.keptStreak", this._origStreak));
                    ModFishing.Instance.Api.SetStreak(this.User, this._origStreak);
                }

                // Invoke fish caught event
                int curFish = this._whichFish.GetValue();
                FishingEventArgs eventArgs = new FishingEventArgs(curFish, this.User, this.User.CurrentTool as FishingRod);
                ModFishing.Instance.Api.OnFishCaught(eventArgs);
                if (eventArgs.ParentSheetIndex != curFish) {
                    this._whichFish.SetValue(eventArgs.ParentSheetIndex);
                }
            }
        }

        public override void emergencyShutDown() {
            // Failed to catch fish
            if (!this._notifiedFailOrSucceed) {
                this._notifiedFailOrSucceed = true;
                ModFishing.Instance.Api.SetStreak(this.User, 0);
                if (this._origStreak >= ModFishing.Instance.MainConfig.StreakSettings.StreakForIncreasedQuality) {
                    Game1.showGlobalMessage(ModFishing.Translate("text.lostStreak", this._origStreak));
                }
            }

            base.emergencyShutDown();
        }

        public override void draw(SpriteBatch b) {
            b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen - (this._flipBubble.GetValue() ? 44 : 20) + 104, this.yPositionOnScreen - 16 + 314) + this._everythingShake.GetValue(), new Rectangle(652, 1685, 52, 157), Color.White * 0.6f * this._scale.GetValue(), 0.0f, new Vector2(26f, 78.5f) * this._scale.GetValue(), 4f * this._scale.GetValue(), this._flipBubble.GetValue() ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f / 1000f);
            b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen + 70, this.yPositionOnScreen + 296) + this._everythingShake.GetValue(), new Rectangle(644, 1999, 37, 150), Color.White * this._scale.GetValue(), 0.0f, new Vector2(18.5f, 74f) * this._scale.GetValue(), 4f * this._scale.GetValue(), SpriteEffects.None, 0.01f);
            if (this._scale.GetValue() >= 1.0) {
                SpriteBatch spriteBatch1 = b;
                Texture2D mouseCursors1 = Game1.mouseCursors;
                Vector2 position1 = new Vector2(this.xPositionOnScreen + 64, this.yPositionOnScreen + 12 + (int) this._bobberBarPos.GetValue()) + this._barShake.GetValue() + this._everythingShake.GetValue();
                Rectangle? sourceRectangle1 = new Rectangle(682, 2078, 9, 2);
                TimeSpan timeOfDay;
                Color color1;
                if (!this._bobberInBar.GetValue()) {
                    Color color2 = Color.White * 0.25f;
                    timeOfDay = DateTime.Now.TimeOfDay;
                    double num = Math.Round(Math.Sin(timeOfDay.TotalMilliseconds / 100.0), 2) + 2.0;
                    color1 = color2 * (float) num;
                } else
                    color1 = Color.White;
                const double num1 = 0.0;
                Vector2 zero1 = Vector2.Zero;
                const double num2 = 4.0;
                const int num3 = 0;
                const double num4 = 0.889999985694885;
                spriteBatch1.Draw(mouseCursors1, position1, sourceRectangle1, color1, (float) num1, zero1, (float) num2, num3, (float) num4);
                SpriteBatch spriteBatch2 = b;
                Texture2D mouseCursors2 = Game1.mouseCursors;
                Vector2 position2 = new Vector2(this.xPositionOnScreen + 64, this.yPositionOnScreen + 12 + (int) this._bobberBarPos.GetValue() + 8) + this._barShake.GetValue() + this._everythingShake.GetValue();
                Rectangle? sourceRectangle2 = new Rectangle(682, 2081, 9, 1);
                Color color3;
                if (!this._bobberInBar.GetValue()) {
                    Color color2 = Color.White * 0.25f;
                    timeOfDay = DateTime.Now.TimeOfDay;
                    double num5 = Math.Round(Math.Sin(timeOfDay.TotalMilliseconds / 100.0), 2) + 2.0;
                    color3 = color2 * (float) num5;
                } else
                    color3 = Color.White;
                const double num6 = 0.0;
                Vector2 zero2 = Vector2.Zero;
                Vector2 scale = new Vector2(4f, this._bobberBarHeight.GetValue() - 16);
                const int num7 = 0;
                const double num8 = 0.889999985694885;
                spriteBatch2.Draw(mouseCursors2, position2, sourceRectangle2, color3, (float) num6, zero2, scale, num7, (float) num8);
                SpriteBatch spriteBatch3 = b;
                Texture2D mouseCursors3 = Game1.mouseCursors;
                Vector2 position3 = new Vector2(this.xPositionOnScreen + 64, this.yPositionOnScreen + 12 + (int) this._bobberBarPos.GetValue() + this._bobberBarHeight.GetValue() - 8) + this._barShake.GetValue() + this._everythingShake.GetValue();
                Rectangle? sourceRectangle3 = new Rectangle(682, 2085, 9, 2);
                Color color4;
                if (!this._bobberInBar.GetValue()) {
                    Color color2 = Color.White * 0.25f;
                    timeOfDay = DateTime.Now.TimeOfDay;
                    double num5 = Math.Round(Math.Sin(timeOfDay.TotalMilliseconds / 100.0), 2) + 2.0;
                    color4 = color2 * (float) num5;
                } else
                    color4 = Color.White;
                const double num9 = 0.0;
                Vector2 zero3 = Vector2.Zero;
                const double num10 = 4.0;
                const int num11 = 0;
                const double num12 = 0.889999985694885;
                spriteBatch3.Draw(mouseCursors3, position3, sourceRectangle3, color4, (float) num9, zero3, (float) num10, num11, (float) num12);
                b.Draw(Game1.staminaRect, new Rectangle(this.xPositionOnScreen + 124, this.yPositionOnScreen + 4 + (int) (580.0 * (1.0 - this._distanceFromCatching.GetValue())), 16, (int) (580.0 * this._distanceFromCatching.GetValue())), Utility.getRedToGreenLerpColor(this._distanceFromCatching.GetValue()));
                b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen + 18, this.yPositionOnScreen + 514) + this._everythingShake.GetValue(), new Rectangle(257, 1990, 5, 10), Color.White, this._reelRotation.GetValue(), new Vector2(2f, 10f), 4f, SpriteEffects.None, 0.9f);
                b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen + 64 + 18, this.yPositionOnScreen + 12 + 24 + this._treasurePosition.GetValue()) + this._treasureShake.GetValue() + this._everythingShake.GetValue(), new Rectangle(638, 1865, 20, 24), Color.White, 0.0f, new Vector2(10f, 10f), 2f * this._treasureScale.GetValue(), SpriteEffects.None, 0.85f);
                if (this._treasureCatchLevel.GetValue() > 0.0 && !this._treasureCaught.GetValue()) {
                    b.Draw(Game1.staminaRect, new Rectangle(this.xPositionOnScreen + 64, this.yPositionOnScreen + 12 + (int) this._treasurePosition.GetValue(), 40, 8), Color.DimGray * 0.5f);
                    b.Draw(Game1.staminaRect, new Rectangle(this.xPositionOnScreen + 64, this.yPositionOnScreen + 12 + (int) this._treasurePosition.GetValue(), (int) (this._treasureCatchLevel.GetValue() * 40.0), 8), Color.Orange);
                }

                // Draw the fish
                Vector2 fishPos = new Vector2(this.xPositionOnScreen + 64 + 18, this.yPositionOnScreen + 12 + 24 + this._bobberPosition.GetValue()) + this._fishShake.GetValue() + this._everythingShake.GetValue();
                if (ModFishing.Instance.MainConfig.ShowFish && !ModFishing.Instance.Api.IsHidden(this._origFish)) {
                    Rectangle fishSrc = GameLocation.getSourceRectForObject(this._origFish);
                    b.Draw(Game1.objectSpriteSheet, fishPos, fishSrc, Color.White, 0.0f, new Vector2(10f, 10f), 2.25f, SpriteEffects.None, 0.88f);
                } else {
                    Rectangle fishSrc = new Rectangle(614 + (this._bossFish.GetValue() ? 20 : 0), 1840, 20, 20);
                    b.Draw(Game1.mouseCursors, fishPos, fishSrc, Color.White, 0.0f, new Vector2(10f, 10f), 2f, SpriteEffects.None, 0.88f);
                }

                // Draw the sparkle text
                this._sparkleText.GetValue()?.draw(b, new Vector2(this.xPositionOnScreen - 16, this.yPositionOnScreen - 64));
            }
            if (Game1.player.fishCaught == null || Game1.player.fishCaught.Count != 0)
                return;
            Vector2 position = new Vector2(this.xPositionOnScreen + (this._flipBubble.GetValue() ? this.width + 64 + 8 : -200), this.yPositionOnScreen + 192);
            if (!Game1.options.gamepadControls)
                b.Draw(Game1.mouseCursors, position, new Rectangle(644, 1330, 48, 69), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
            else
                b.Draw(Game1.controllerMaps, position, Utility.controllerMapSourceRect(new Rectangle(681, 0, 96, 138)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
        }
    }
}
