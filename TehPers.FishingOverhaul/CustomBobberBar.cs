using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Tools;
using TehPers.Core;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul {
    public class CustomBobberBar : BobberBar {
        private readonly ReflectedField<BobberBar, bool> _treasure;
        private readonly ReflectedField<BobberBar, bool> _treasureCaught;
        private readonly ReflectedField<BobberBar, float> _treasurePosition;
        private readonly ReflectedField<BobberBar, float> _treasureAppearTimer;
        private readonly ReflectedField<BobberBar, float> _treasureScale;

        private readonly ReflectedField<BobberBar, float> _distanceFromCatching;
        private readonly ReflectedField<BobberBar, float> _treasureCatchLevel;

        private readonly ReflectedField<BobberBar, float> _bobberBarPos;
        private readonly ReflectedField<BobberBar, bool> _bobberInBar;
        private readonly ReflectedField<BobberBar, float> _difficulty;
        private readonly ReflectedField<BobberBar, int> _fishQuality;
        private readonly ReflectedField<BobberBar, bool> _perfect;
        private readonly ReflectedField<BobberBar, float> _scale;
        private readonly ReflectedField<BobberBar, bool> _flipBubble;
        private readonly ReflectedField<BobberBar, int> _bobberBarHeight;
        private readonly ReflectedField<BobberBar, float> _reelRotation;
        private readonly ReflectedField<BobberBar, float> _bobberPosition;
        private readonly ReflectedField<BobberBar, bool> _bossFish;
        private readonly ReflectedField<BobberBar, int> _motionType;
        private readonly ReflectedField<BobberBar, int> _fishSize;
        private readonly ReflectedField<BobberBar, int> _whichFish;

        private readonly ReflectedField<BobberBar, Vector2> _barShake;
        private readonly ReflectedField<BobberBar, Vector2> _fishShake;
        private readonly ReflectedField<BobberBar, Vector2> _treasureShake;
        private readonly ReflectedField<BobberBar, Vector2> _everythingShake;

        private readonly ReflectedField<BobberBar, SparklingText> _sparkleText;

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

        public CustomBobberBar(Farmer user, int whichFish, float fishSize, bool treasure, int bobber) : base(whichFish, fishSize, treasure, bobber) {
            User = user;
            _origStreak = ModFishing.Instance.Api.GetStreak(user);
            _origFish = whichFish;

            /* Private field hooks */
            _treasure = new ReflectedField<BobberBar, bool>(this, "treasure");
            _treasure = new ReflectedField<BobberBar, bool>(this, "treasure");
            _treasureCaught = new ReflectedField<BobberBar, bool>(this, "treasureCaught");
            _treasurePosition = new ReflectedField<BobberBar, float>(this, "treasurePosition");
            _treasureAppearTimer = new ReflectedField<BobberBar, float>(this, "treasureAppearTimer");
            _treasureScale = new ReflectedField<BobberBar, float>(this, "treasureScale");

            _distanceFromCatching = new ReflectedField<BobberBar, float>(this, "distanceFromCatching");
            _treasureCatchLevel = new ReflectedField<BobberBar, float>(this, "treasureCatchLevel");

            _bobberBarPos = new ReflectedField<BobberBar, float>(this, "bobberBarPos");
            _bobberInBar = new ReflectedField<BobberBar, bool>(this, "bobberInBar");
            _difficulty = new ReflectedField<BobberBar, float>(this, "difficulty");
            _fishQuality = new ReflectedField<BobberBar, int>(this, "fishQuality");
            _perfect = new ReflectedField<BobberBar, bool>(this, "perfect");
            _scale = new ReflectedField<BobberBar, float>(this, "scale");
            _flipBubble = new ReflectedField<BobberBar, bool>(this, "flipBubble");
            _bobberBarHeight = new ReflectedField<BobberBar, int>(this, "bobberBarHeight");
            _reelRotation = new ReflectedField<BobberBar, float>(this, "reelRotation");
            _bobberPosition = new ReflectedField<BobberBar, float>(this, "bobberPosition");
            _bossFish = new ReflectedField<BobberBar, bool>(this, "bossFish");
            _motionType = new ReflectedField<BobberBar, int>(this, "motionType");
            _fishSize = new ReflectedField<BobberBar, int>(this, "fishSize");
            _whichFish = new ReflectedField<BobberBar, int>(this, "whichFish");

            _barShake = new ReflectedField<BobberBar, Vector2>(this, "barShake");
            _fishShake = new ReflectedField<BobberBar, Vector2>(this, "fishShake");
            _treasureShake = new ReflectedField<BobberBar, Vector2>(this, "treasureShake");
            _everythingShake = new ReflectedField<BobberBar, Vector2>(this, "everythingShake");

            _sparkleText = new ReflectedField<BobberBar, SparklingText>(this, "sparkleText");

            _lastDistanceFromCatching = _distanceFromCatching.Value;
            _lastTreasureCatchLevel = _treasureCatchLevel.Value;

            /* Actual code */
            var config = ModFishing.Instance.MainConfig;
            var traits = ModFishing.Instance.Api.GetFishTraits(whichFish);

            // Check if fish is unaware
            Unaware = Game1.random.NextDouble() < ModFishing.Instance.Api.GetUnawareChance(user, whichFish);

            // Applies difficulty modifier, including if fish is unaware
            var difficulty = traits?.Difficulty ?? _difficulty.Value;
            difficulty *= config.DifficultySettings.BaseDifficultyMult;
            difficulty *= 1F + _origStreak * config.DifficultySettings.DifficultyStreakEffect;
            if (Unaware) {
                difficulty *= config.UnawareSettings.UnawareMult;
                Game1.showGlobalMessage(ModFishing.Translate("text.unaware", ModFishing.Translate("text.percent", 1F - config.UnawareSettings.UnawareMult)));
            }

            _difficulty.Value = difficulty;

            // Adjusts additional traits about the fish
            if (traits != null) {
                _motionType.Value = (int) traits.MotionType;
                _fishSize.Value = traits.MinSize + (int) ((traits.MaxSize - traits.MinSize) * fishSize) + 1;
            }

            // Adjusts quality to be increased by streak
            var fishQuality = _fishQuality.Value;
            _origQuality = fishQuality;
            var qualityBonus = (int) Math.Floor((double) _origStreak / config.StreakSettings.StreakForIncreasedQuality);
            fishQuality = Math.Min(fishQuality + qualityBonus, 3);
            if (fishQuality == 3) fishQuality++; // Iridium-quality fish. Only possible through your perfect streak
            _fishQuality.Value = fishQuality;

            // Increase the user's perfect streak (this will be dropped to 0 if they don't get a perfect catch)
            if (_origStreak >= config.StreakSettings.StreakForIncreasedQuality) _sparkleText.Value = new SparklingText(Game1.dialogueFont, ModFishing.Translate("text.streak", _origStreak), Color.Yellow, Color.White);
        }

        public override void update(GameTime time) {
            // Speed warp on catching fish
            var distanceFromCatching = _distanceFromCatching.Value;
            var delta = distanceFromCatching - _lastDistanceFromCatching;
            var mult = delta > 0 ? ModFishing.Instance.MainConfig.DifficultySettings.CatchSpeed : ModFishing.Instance.MainConfig.DifficultySettings.DrainSpeed;
            distanceFromCatching = _lastDistanceFromCatching + delta * mult;
            _lastDistanceFromCatching = distanceFromCatching;
            _distanceFromCatching.Value = distanceFromCatching;

            // Speed warp on catching treasure
            var treasureCatchLevel = _treasureCatchLevel.Value;
            delta = treasureCatchLevel - _lastTreasureCatchLevel;
            mult = delta > 0 ? ModFishing.Instance.MainConfig.DifficultySettings.TreasureCatchSpeed : ModFishing.Instance.MainConfig.DifficultySettings.TreasureDrainSpeed;
            treasureCatchLevel = _lastTreasureCatchLevel + delta * mult;
            _lastTreasureCatchLevel = treasureCatchLevel;
            _treasureCatchLevel.Value = treasureCatchLevel;

            var perfect = _perfect.Value;
            var treasure = _treasure.Value;
            var treasureCaught = _treasureCaught.Value;

            // Check if still perfect, otherwise apply changes to loot
            if (!_perfectChanged && !perfect) {
                _perfectChanged = true;
                _fishQuality.Value = Math.Min(_origQuality, ModFishing.Instance.MainConfig.DifficultySettings.PreventGoldOnNormalCatch ? 1 : 2);
                ModFishing.Instance.Api.SetStreak(User, 0);
                if (_origStreak >= ModFishing.Instance.MainConfig.StreakSettings.StreakForIncreasedQuality) {
                    Game1.showGlobalMessage(ModFishing.Translate(treasure ? "text.warnStreak" : "text.lostStreak", _origStreak));
                }
            }

            // Check if lost perfect, but got treasure
            if (!_treasureChanged && !perfect && treasure && treasureCaught) {
                _treasureChanged = true;
                var qualityBonus = (int) Math.Floor((double) _origStreak / ModFishing.Instance.MainConfig.StreakSettings.StreakForIncreasedQuality);
                var quality = _origQuality;
                quality = Math.Min(quality + qualityBonus, 3);
                if (quality == 3) quality++;
                _fishQuality.Value = quality;
            }

            // Base call
            base.update(time);

            // Check if done fishing
            distanceFromCatching = _distanceFromCatching.Value;
            if (_notifiedFailOrSucceed) return;

            if (distanceFromCatching <= 0.0) {
                // Failed to catch fish
                _notifiedFailOrSucceed = true;

                if (!treasure) return;
                _notifiedFailOrSucceed = true;
                if (_origStreak >= ModFishing.Instance.MainConfig.StreakSettings.StreakForIncreasedQuality) {
                    Game1.showGlobalMessage(ModFishing.Translate("text.lostStreak", _origStreak));
                }
            }
            else if (distanceFromCatching >= 1.0) {
                // Succeeded in catching the fish
                _notifiedFailOrSucceed = true;

                if (perfect) {
                    ModFishing.Instance.Api.SetStreak(User, _origStreak + 1);
                }
                else if (treasure && treasureCaught) {
                    if (_origStreak >= ModFishing.Instance.MainConfig.StreakSettings.StreakForIncreasedQuality) Game1.showGlobalMessage(ModFishing.Translate("text.keptStreak", _origStreak));
                    ModFishing.Instance.Api.SetStreak(User, _origStreak);
                }

                // Invoke fish caught event
                var curFish = _whichFish.Value;
                var eventArgs = new FishingEventArgs(curFish, User, User.CurrentTool as FishingRod);
                ModFishing.Instance.Api.OnFishCaught(eventArgs);
                if (eventArgs.ParentSheetIndex != curFish) {
                    _whichFish.Value = eventArgs.ParentSheetIndex;
                }
            }
        }

        public override void emergencyShutDown() {
            // Failed to catch fish
            if (!_notifiedFailOrSucceed) {
                _notifiedFailOrSucceed = true;
                ModFishing.Instance.Api.SetStreak(User, 0);
                if (_origStreak >= ModFishing.Instance.MainConfig.StreakSettings.StreakForIncreasedQuality) {
                    Game1.showGlobalMessage(ModFishing.Translate("text.lostStreak", _origStreak));
                }
            }

            base.emergencyShutDown();
        }

        public override void draw(SpriteBatch b) {
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - (_flipBubble.Value ? 44 : 20) + 104, yPositionOnScreen - 16 + 314) + _everythingShake.Value, new Rectangle(652, 1685, 52, 157), Color.White * 0.6f * _scale.Value, 0.0f, new Vector2(26f, 78.5f) * _scale.Value, 4f * _scale.Value, _flipBubble.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f / 1000f);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen + 70, yPositionOnScreen + 296) + _everythingShake.Value, new Rectangle(644, 1999, 37, 150), Color.White * _scale.Value, 0.0f, new Vector2(18.5f, 74f) * _scale.Value, 4f * _scale.Value, SpriteEffects.None, 0.01f);
            if (_scale.Value >= 1.0) {
                var spriteBatch1 = b;
                var mouseCursors1 = Game1.mouseCursors;
                var position1 = new Vector2(xPositionOnScreen + 64, yPositionOnScreen + 12 + (int) _bobberBarPos.Value) + _barShake.Value + _everythingShake.Value;
                Rectangle? sourceRectangle1 = new Rectangle(682, 2078, 9, 2);
                TimeSpan timeOfDay;
                Color color1;
                if (!_bobberInBar.Value) {
                    var color2 = Color.White * 0.25f;
                    timeOfDay = DateTime.Now.TimeOfDay;
                    var num = Math.Round(Math.Sin(timeOfDay.TotalMilliseconds / 100.0), 2) + 2.0;
                    color1 = color2 * (float) num;
                }
                else
                    color1 = Color.White;

                const double num1 = 0.0;
                var zero1 = Vector2.Zero;
                const double num2 = 4.0;
                const int num3 = 0;
                const double num4 = 0.889999985694885;
                spriteBatch1.Draw(mouseCursors1, position1, sourceRectangle1, color1, (float) num1, zero1, (float) num2, num3, (float) num4);
                var spriteBatch2 = b;
                var mouseCursors2 = Game1.mouseCursors;
                var position2 = new Vector2(xPositionOnScreen + 64, yPositionOnScreen + 12 + (int) _bobberBarPos.Value + 8) + _barShake.Value + _everythingShake.Value;
                Rectangle? sourceRectangle2 = new Rectangle(682, 2081, 9, 1);
                Color color3;
                if (!_bobberInBar.Value) {
                    var color2 = Color.White * 0.25f;
                    timeOfDay = DateTime.Now.TimeOfDay;
                    var num5 = Math.Round(Math.Sin(timeOfDay.TotalMilliseconds / 100.0), 2) + 2.0;
                    color3 = color2 * (float) num5;
                }
                else
                    color3 = Color.White;

                const double num6 = 0.0;
                var zero2 = Vector2.Zero;
                var scale = new Vector2(4f, _bobberBarHeight.Value - 16);
                const int num7 = 0;
                const double num8 = 0.889999985694885;
                spriteBatch2.Draw(mouseCursors2, position2, sourceRectangle2, color3, (float) num6, zero2, scale, num7, (float) num8);
                var spriteBatch3 = b;
                var mouseCursors3 = Game1.mouseCursors;
                var position3 = new Vector2(xPositionOnScreen + 64, yPositionOnScreen + 12 + (int) _bobberBarPos.Value + _bobberBarHeight.Value - 8) + _barShake.Value + _everythingShake.Value;
                Rectangle? sourceRectangle3 = new Rectangle(682, 2085, 9, 2);
                Color color4;
                if (!_bobberInBar.Value) {
                    var color2 = Color.White * 0.25f;
                    timeOfDay = DateTime.Now.TimeOfDay;
                    var num5 = Math.Round(Math.Sin(timeOfDay.TotalMilliseconds / 100.0), 2) + 2.0;
                    color4 = color2 * (float) num5;
                }
                else
                    color4 = Color.White;

                const double num9 = 0.0;
                var zero3 = Vector2.Zero;
                const double num10 = 4.0;
                const int num11 = 0;
                const double num12 = 0.889999985694885;
                spriteBatch3.Draw(mouseCursors3, position3, sourceRectangle3, color4, (float) num9, zero3, (float) num10, num11, (float) num12);
                b.Draw(Game1.staminaRect, new Rectangle(xPositionOnScreen + 124, yPositionOnScreen + 4 + (int) (580.0 * (1.0 - _distanceFromCatching.Value)), 16, (int) (580.0 * _distanceFromCatching.Value)), Utility.getRedToGreenLerpColor(_distanceFromCatching.Value));
                b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen + 18, yPositionOnScreen + 514) + _everythingShake.Value, new Rectangle(257, 1990, 5, 10), Color.White, _reelRotation.Value, new Vector2(2f, 10f), 4f, SpriteEffects.None, 0.9f);
                b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen + 64 + 18, yPositionOnScreen + 12 + 24 + _treasurePosition.Value) + _treasureShake.Value + _everythingShake.Value, new Rectangle(638, 1865, 20, 24), Color.White, 0.0f, new Vector2(10f, 10f), 2f * _treasureScale.Value, SpriteEffects.None, 0.85f);
                if (_treasureCatchLevel.Value > 0.0 && !_treasureCaught.Value) {
                    b.Draw(Game1.staminaRect, new Rectangle(xPositionOnScreen + 64, yPositionOnScreen + 12 + (int) _treasurePosition.Value, 40, 8), Color.DimGray * 0.5f);
                    b.Draw(Game1.staminaRect, new Rectangle(xPositionOnScreen + 64, yPositionOnScreen + 12 + (int) _treasurePosition.Value, (int) (_treasureCatchLevel.Value * 40.0), 8), Color.Orange);
                }

                // Draw the fish
                var fishPos = new Vector2(xPositionOnScreen + 64 + 18, yPositionOnScreen + 12 + 24 + _bobberPosition.Value) + _fishShake.Value + _everythingShake.Value;
                if (ModFishing.Instance.MainConfig.ShowFish && !ModFishing.Instance.Api.IsHidden(_origFish)) {
                    var fishSrc = GameLocation.getSourceRectForObject(_origFish);
                    b.Draw(Game1.objectSpriteSheet, fishPos, fishSrc, Color.White, 0.0f, new Vector2(10f, 10f), 2.25f, SpriteEffects.None, 0.88f);
                }
                else {
                    var fishSrc = new Rectangle(614 + (_bossFish.Value ? 20 : 0), 1840, 20, 20);
                    b.Draw(Game1.mouseCursors, fishPos, fishSrc, Color.White, 0.0f, new Vector2(10f, 10f), 2f, SpriteEffects.None, 0.88f);
                }

                // Draw the sparkle text
                _sparkleText.Value?.draw(b, new Vector2(xPositionOnScreen - 16, yPositionOnScreen - 64));
            }

            if (Game1.player.fishCaught == null || Game1.player.fishCaught.FieldDict.Count != 0) return;
            var position = new Vector2(xPositionOnScreen + (_flipBubble.Value ? width + 64 + 8 : -200), yPositionOnScreen + 192);
            if (!Game1.options.gamepadControls)
                b.Draw(Game1.mouseCursors, position, new Rectangle(644, 1330, 48, 69), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
            else
                b.Draw(Game1.controllerMaps, position, Utility.controllerMapSourceRect(new Rectangle(681, 0, 96, 138)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
        }
    }
}