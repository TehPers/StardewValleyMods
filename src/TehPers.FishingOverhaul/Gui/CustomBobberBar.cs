using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using StardewValley.Tools;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Extensions;
using TehPers.FishingOverhaul.Extensions.Drawing;
using System.Collections.Generic;

namespace TehPers.FishingOverhaul.Gui
{
    internal sealed class CustomBobberBar : BobberBar
    {
        private readonly IReflectedField<bool> perfectField;

        private readonly IReflectedField<SparklingText?> sparkleTextField;

        private readonly FishEntry fishEntry;
        private readonly Item fishItem;
        private readonly FishTraits fishTraits;
        private readonly FishConfig fishConfig;
        private readonly TreasureConfig treasureConfig;
        private readonly FishingInfo fishingInfo;

        private float lastDistanceFromCatching;
        private float lastTreasureCatchLevel;
        private MinigameState state;
        private bool notifiedCatch;

        /// <summary>
        /// Invoked whenever a fish is caught.
        /// </summary>
        public event EventHandler<CatchInfo.FishCatch>? CatchFish;

        /// <summary>
        /// Invoked whenever a perfect streak is lost.
        /// </summary>
        public event EventHandler<MinigameState>? StateChanged;

        /// <summary>
        /// Invoked whenever the fish is not caught.
        /// </summary>
        public event EventHandler? LostFish;

        public CustomBobberBar(
            IModHelper helper,
            FishConfig fishConfig,
            TreasureConfig treasureConfig,
            FishingInfo fishingInfo,
            FishEntry fishEntry,
            FishTraits fishTraits,
            Item fishItem,
            float fishSizePercent,
            bool treasure,
            List<string> bobbers,
            bool fromFishPond,
            bool isBossFish = false
        )
            : base("0", fishSizePercent, treasure, bobbers, fishingInfo.SetFlagOnCatch, isBossFish)
        {
            _ = helper ?? throw new ArgumentNullException(nameof(helper));
            this.fishConfig = fishConfig ?? throw new ArgumentNullException(nameof(fishConfig));
            this.treasureConfig =
                treasureConfig ?? throw new ArgumentNullException(nameof(treasureConfig));
            this.fishingInfo = fishingInfo ?? throw new ArgumentNullException(nameof(fishingInfo));
            this.fishEntry = fishEntry;
            this.fishTraits = fishTraits ?? throw new ArgumentNullException(nameof(fishTraits));
            this.fishItem = fishItem ?? throw new ArgumentNullException(nameof(fishItem));

            this.perfectField = helper.Reflection.GetField<bool>(this, "perfect");
            this.sparkleTextField = helper.Reflection.GetField<SparklingText?>(this, "sparkleText");

            // Track state
            this.lastDistanceFromCatching = 0f;
            this.lastTreasureCatchLevel = 0f;
            this.state = new(true, treasure ? TreasureState.NotCaught : TreasureState.None);

            // Track player streak
            this.perfectField.SetValue(true);
            this.fishSizeReductionTimer = 800;

            // Fish size
            var minFishSize = fishTraits.MinSize;
            var maxFishSize = fishTraits.MaxSize;
            var fishSize = (int)(minFishSize + (maxFishSize - minFishSize) * fishSizePercent) + 1;
            this.minFishSize = minFishSize;
            this.fishSize = fishSize;

            // Track other information (not all tracked by vanilla)
            this.fromFishPond = fromFishPond;
            this.bossFish = fishTraits.IsLegendary;

            // Adjust quality to be increased by streak
            this.fishQuality = fishSizePercent switch
            {
                < 0.33f => 0,
                < 0.66f => 1,
                _ => 2,
            };

            // Quality bobber
            if (bobbers.Contains("877"))
            {
                this.fishQuality += 1;
                if (this.fishQuality > 2)
                {
                    this.fishQuality += 1;
                }
            }

            // Beginner rod
            if (fishingInfo.User.CurrentTool is FishingRod { UpgradeLevel: 1 })
            {
                this.fishQuality = 0;
            }

            // Adjust fish difficulty
            this.difficulty = fishTraits.DartFrequency;
            this.motionType = fishTraits.DartBehavior switch
            {
                DartBehavior.Mixed => BobberBar.mixed,
                DartBehavior.Dart => BobberBar.dart,
                DartBehavior.Smooth => BobberBar.smooth,
                DartBehavior.Sink => BobberBar.sink,
                DartBehavior.Floater => BobberBar.floater,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(fishTraits),
                    "Invalid dart behavior."
                )
            };
        }

        public override void update(GameTime time)
        {
            // Speed warp on catching fish
            var delta = this.distanceFromCatching - this.lastDistanceFromCatching;
            var mult = delta switch
            {
                > 0f => this.fishConfig.CatchSpeed,
                < 0f => this.fishConfig.DrainSpeed,
                _ => 0f,
            };
            this.distanceFromCatching = this.lastDistanceFromCatching + delta * mult;
            this.lastDistanceFromCatching = this.distanceFromCatching;

            // Speed warp on catching treasure
            delta = this.treasureCatchLevel - this.lastTreasureCatchLevel;
            mult = delta switch
            {
                > 0f => this.treasureConfig.CatchSpeed,
                < 0f => this.treasureConfig.DrainSpeed,
                _ => 0f,
            };
            this.treasureCatchLevel = this.lastTreasureCatchLevel + delta * mult;
            this.lastTreasureCatchLevel = this.treasureCatchLevel;

            var perfect = this.perfectField.GetValue();

            // Update state
            var newState = new MinigameState(
                perfect,
                (this.treasure, this.treasureCaught) switch
                {
                    (false, _) => TreasureState.None,
                    (_, false) => TreasureState.NotCaught,
                    (_, true) => TreasureState.Caught,
                }
            );
            if (this.state != newState)
            {
                this.OnStateChanged(newState);
                this.state = newState;
            }

            // Override post-catch logic
            if (this.fadeOut)
            {
                if (this.scale <= 0.05f)
                {
                    // Check for wild bait
                    var caughtDouble = !this.bossFish
                        && Game1.player.CurrentTool is FishingRod { attachments: { } attachments }
                        && attachments[0]?.ParentSheetIndex is 774
                        && Game1.random.NextDouble() < 0.25 + Game1.player.DailyLuck / 2.0;
                    if (this.distanceFromCatching > 0.9 && Game1.player.CurrentTool is FishingRod)
                    {
                        // Notify that a fish was caught
                        var catchInfo = new CatchInfo.FishCatch(
                            this.fishingInfo,
                            this.fishEntry,
                            this.fishItem,
                            this.fishSize,
                            this.fishTraits.IsLegendary,
                            this.fishQuality,
                            (int)this.difficulty,
                            this.state,
                            this.fromFishPond,
                            caughtDouble ? Math.Max(this.challengeBaitFishes, 2) : 1
                        );
                        this.OnCaughtFish(catchInfo);
                    }
                    else
                    {
                        Game1.player.completelyStopAnimatingOrDoingAction();
                        if (Game1.player.CurrentTool is FishingRod rod)
                        {
                            rod.doneFishing(Game1.player, true);
                        }

                        this.OnLostFish();
                    }

                    Game1.exitActiveMenu();
                    Game1.setRichPresence("location", Game1.currentLocation.Name);
                    return;
                }
            }

            // Base call
            base.update(time);
        }

        public override void emergencyShutDown()
        {
            if (this.distanceFromCatching <= 0.9)
            {
                // Failed to catch fish
                this.OnLostFish();
            }

            base.emergencyShutDown();
        }

        public override void draw(SpriteBatch b)
        {
            Game1.StartWorldDrawInUI(b);

            b.Draw(
                Game1.mouseCursors,
                new Vector2(
                    this.xPositionOnScreen - (this.flipBubble ? 44 : 20) + 104,
                    this.yPositionOnScreen - 16 + 314
                )
                + this.everythingShake,
                new Rectangle(652, 1685, 52, 157),
                Color.White * 0.6f * this.scale,
                0.0f,
                new Vector2(26f, 78.5f) * this.scale,
                4f * this.scale,
                this.flipBubble ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                1f / 1000f
            );
            b.Draw(
                Game1.mouseCursors,
                new Vector2(this.xPositionOnScreen + 70, this.yPositionOnScreen + 296)
                + this.everythingShake,
                new Rectangle(644, 1999, 37, 150),
                Color.White * this.scale,
                0.0f,
                new Vector2(18.5f, 74f) * this.scale,
                4f * this.scale,
                SpriteEffects.None,
                0.01f
            );

            if (Math.Abs(this.scale - 1.0) < 0.001)
            {
                var color = this.bobberInBar
                    ? Color.White
                    : Color.White
                    * 0.25f
                    * ((float)Math.Round(
                            Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 100.0),
                            2
                        )
                        + 2f);

                // Draw background and components
                b.Draw(
                    Game1.mouseCursors,
                    new Vector2(
                        this.xPositionOnScreen + 64,
                        this.yPositionOnScreen + 12 + (int)this.bobberBarPos
                    )
                    + this.barShake
                    + this.everythingShake,
                    new Rectangle(682, 2078, 9, 2),
                    color,
                    0.0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    0.89f
                );
                b.Draw(
                    Game1.mouseCursors,
                    new Vector2(
                        this.xPositionOnScreen + 64,
                        this.yPositionOnScreen + 12 + (int)this.bobberBarPos + 8
                    )
                    + this.barShake
                    + this.everythingShake,
                    new Rectangle(682, 2081, 9, 1),
                    color,
                    0.0f,
                    Vector2.Zero,
                    new Vector2(4f, this.bobberBarHeight - 16),
                    SpriteEffects.None,
                    0.89f
                );
                b.Draw(
                    Game1.mouseCursors,
                    new Vector2(
                        this.xPositionOnScreen + 64,
                        this.yPositionOnScreen + 12 + (int)this.bobberBarPos + this.bobberBarHeight - 8
                    )
                    + this.barShake
                    + this.everythingShake,
                    new Rectangle(682, 2085, 9, 2),
                    color,
                    0.0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    0.89f
                );
                b.Draw(
                    Game1.staminaRect,
                    new Rectangle(
                        this.xPositionOnScreen + 124,
                        this.yPositionOnScreen + 4 + (int)(580.0 * (1.0 - this.distanceFromCatching)),
                        16,
                        (int)(580.0 * this.distanceFromCatching)
                    ),
                    Utility.getRedToGreenLerpColor(this.distanceFromCatching)
                );
                b.Draw(
                    Game1.mouseCursors,
                    new Vector2(this.xPositionOnScreen + 18, this.yPositionOnScreen + 514)
                    + this.everythingShake,
                    new Rectangle(257, 1990, 5, 10),
                    Color.White,
                    this.reelRotation,
                    new(2f, 10f),
                    4f,
                    SpriteEffects.None,
                    0.9f
                );

                // Draw treasure
                b.Draw(
                    Game1.mouseCursors,
                    new Vector2(
                        this.xPositionOnScreen + 64 + 18,
                        this.yPositionOnScreen + 12 + 24 + this.treasurePosition
                    )
                    + this.treasureShake
                    + this.everythingShake,
                    new Rectangle(638, 1865, 20, 24),
                    Color.White,
                    0.0f,
                    new(10f, 10f),
                    2f * this.treasureScale,
                    SpriteEffects.None,
                    0.85f
                );
                if (this.treasureCatchLevel > 0.0 && !this.treasureCaught)
                {
                    b.Draw(
                        Game1.staminaRect,
                        new Rectangle(
                            this.xPositionOnScreen + 64,
                            this.yPositionOnScreen + 12 + (int)this.treasurePosition,
                            40,
                            8
                        ),
                        Color.DimGray * 0.5f
                    );
                    b.Draw(
                        Game1.staminaRect,
                        new Rectangle(
                            this.xPositionOnScreen + 64,
                            this.yPositionOnScreen + 12 + (int)this.treasurePosition,
                            (int)(this.treasureCatchLevel * 40.0),
                            8
                        ),
                        Color.Orange
                    );
                }

                // Draw fish
                var position = new Vector2(
                    this.xPositionOnScreen + 64 + 18,
                    this.yPositionOnScreen + this.bobberPosition + 12 + 24
                );
                if (this.fishConfig.ShowFishInMinigame)
                {
                    this.fishItem.DrawInMenuCorrected(
                        b,
                        position + this.fishShake + this.everythingShake,
                        0.5f,
                        1f,
                        0.88f,
                        StackDrawType.Hide,
                        Color.White,
                        false,
                        new CenterDrawOrigin()
                    );
                }
                else
                {
                    b.Draw(
                        Game1.mouseCursors,
                        position + this.fishShake + this.everythingShake,
                        new Rectangle(614 + (this.fishTraits.IsLegendary ? 20 : 0), 1840, 20, 20),
                        Color.White,
                        0.0f,
                        new(10f, 10f),
                        2f,
                        SpriteEffects.None,
                        0.88f
                    );
                }

                // Draw sparkle text
                var sparkleText = this.sparkleTextField.GetValue();
                sparkleText?.draw(b, new(this.xPositionOnScreen - 16, this.yPositionOnScreen - 64));
            }

            if (Game1.player.fishCaught?.Any() == false)
            {
                var position = new Vector2(
                    this.xPositionOnScreen + (this.flipBubble ? this.width + 64 + 8 : -200),
                    this.yPositionOnScreen + 192
                );
                if (!Game1.options.gamepadControls)
                {
                    b.Draw(
                        Game1.mouseCursors,
                        position,
                        new Rectangle(644, 1330, 48, 69),
                        Color.White,
                        0.0f,
                        Vector2.Zero,
                        4f,
                        SpriteEffects.None,
                        0.88f
                    );
                }
                else
                {
                    b.Draw(
                        Game1.controllerMaps,
                        position,
                        Utility.controllerMapSourceRect(new(681, 0, 96, 138)),
                        Color.White,
                        0.0f,
                        Vector2.Zero,
                        2f,
                        SpriteEffects.None,
                        0.88f
                    );
                }
            }

            Game1.EndWorldDrawInUI(b);
        }

        private void OnCaughtFish(CatchInfo.FishCatch e)
        {
            if (this.notifiedCatch)
            {
                return;
            }

            this.notifiedCatch = true;
            this.CatchFish?.Invoke(this, e);
        }

        private void OnLostFish()
        {
            if (this.notifiedCatch)
            {
                return;
            }

            this.notifiedCatch = true;
            this.LostFish?.Invoke(this, EventArgs.Empty);
        }

        private void OnStateChanged(MinigameState e)
        {
            this.StateChanged?.Invoke(this, e);
        }
    }
}
