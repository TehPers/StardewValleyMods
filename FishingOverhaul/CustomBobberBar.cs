using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using TehPers.Stardew.FishingOverhaul.Configs;
using static TehPers.Stardew.FishingOverhaul.Configs.ConfigFish;
using System.Linq;
using TehPers.Stardew.Framework;
using StardewValley.Tools;

namespace TehPers.Stardew.FishingOverhaul {
    public class CustomBobberBar : BobberBar {

        private IPrivateField<bool> treasureField;
        private IPrivateField<bool> treasureCaughtField;
        private IPrivateField<float> treasurePositionField;
        private IPrivateField<float> treasureAppearTimerField;
        private IPrivateField<float> treasureScaleField;

        private IPrivateField<float> distanceFromCatchingField;
        private IPrivateField<float> treasureCatchLevelField;

        private IPrivateField<float> bobberBarPosField;
        private IPrivateField<float> difficultyField;
        private IPrivateField<int> fishQualityField;
        private IPrivateField<bool> perfectField;

        private IPrivateField<SparklingText> sparkleTextField;

        private float lastDistanceFromCatching;
        private float lastTreasureCatchLevel;
        private bool perfectChanged = false;
        private bool treasureChanged = false;
        private bool notifiedFailOrSucceed = false;
        private int origStreak;
        private int origQuality;

        private bool exitImmediately;

        public Farmer user;

        public CustomBobberBar(Farmer user, int whichFish, float fishSize, bool treasure, int bobber, int waterDepth) : base(whichFish, fishSize, treasure, bobber) {
            this.user = user;
            this.origStreak = FishHelper.getStreak(user);

            /* Private field hooks */
            treasureField = ModEntry.INSTANCE.Helper.Reflection.GetPrivateField<bool>(this, "treasure");
            treasureCaughtField = ModEntry.INSTANCE.Helper.Reflection.GetPrivateField<bool>(this, "treasureCaught");
            treasurePositionField = ModEntry.INSTANCE.Helper.Reflection.GetPrivateField<float>(this, "treasurePosition");
            treasureAppearTimerField = ModEntry.INSTANCE.Helper.Reflection.GetPrivateField<float>(this, "treasureAppearTimer");
            treasureScaleField = ModEntry.INSTANCE.Helper.Reflection.GetPrivateField<float>(this, "treasureScale");

            distanceFromCatchingField = ModEntry.INSTANCE.Helper.Reflection.GetPrivateField<float>(this, "distanceFromCatching");
            treasureCatchLevelField = ModEntry.INSTANCE.Helper.Reflection.GetPrivateField<float>(this, "treasureCatchLevel");

            bobberBarPosField = ModEntry.INSTANCE.Helper.Reflection.GetPrivateField<float>(this, "bobberBarPos");
            difficultyField = ModEntry.INSTANCE.Helper.Reflection.GetPrivateField<float>(this, "difficulty");
            fishQualityField = ModEntry.INSTANCE.Helper.Reflection.GetPrivateField<int>(this, "fishQuality");
            perfectField = ModEntry.INSTANCE.Helper.Reflection.GetPrivateField<bool>(this, "perfect");
            
            sparkleTextField = ModEntry.INSTANCE.Helper.Reflection.GetPrivateField<SparklingText>(this, "sparkleText");

            lastDistanceFromCatching = distanceFromCatchingField.GetValue();
            lastTreasureCatchLevel = treasureCatchLevelField.GetValue();

            /* Actual code */
            ConfigMain config = ModEntry.INSTANCE.config;

            // Choose a random fish, this time using the custom fish selector
            FishingRod rod = Game1.player.CurrentTool as FishingRod;
            //int waterDepth = rod != null ? ModEntry.INSTANCE.Helper.Reflection.GetPrivateValue<int>(rod, "clearWaterDistance") : 0;

            // Applies difficulty modifier, including if fish isn't paying attention
            float difficulty = difficultyField.GetValue() * config.BaseDifficultyMult;
            difficulty *= 1f + config.DifficultyStreakEffect * this.origStreak;
            double difficultyChance = config.UnawareChance + user.LuckLevel * config.UnawareLuckLevelEffect + Game1.dailyLuck * config.UnawareDailyLuckEffect;
            if (Game1.random.NextDouble() < difficultyChance) {
                Game1.showGlobalMessage("The fish is unaware of your presence. (" + ((1f - config.UnawareMult) * 100) + "% easier catch)");
                difficulty *= config.UnawareMult;
            }
            difficultyField.SetValue(difficulty);

            // Adjusts quality to be increased by streak
            int fishQuality = fishQualityField.GetValue();
            this.origQuality = fishQuality;
            int qualityBonus = (int) Math.Floor((double) this.origStreak / config.StreakForIncreasedQuality);
            fishQuality = Math.Min(fishQuality + qualityBonus, 3);
            if (fishQuality == 3) fishQuality++; // Iridium-quality fish. Only possible through your perfect streak
            fishQualityField.SetValue(fishQuality);

            // Increase the user's perfect streak (this will be dropped to 0 if they don't get a perfect catch)
            if (this.origStreak >= config.StreakForIncreasedQuality)
                sparkleTextField.SetValue(new SparklingText(Game1.dialogueFont, "Streak: " + this.origStreak, Color.Yellow, Color.White, false, 0.1, 2500, -1, 500));
            FishHelper.setStreak(user, this.origStreak + 1);
        }

        public override void update(GameTime time) {
            if (exitImmediately) {
                Game1.exitActiveMenu();
                return;
            }

            // Speed warp on normal catching
            float distanceFromCatching = distanceFromCatchingField.GetValue();
            float delta = distanceFromCatching - this.lastDistanceFromCatching;
            distanceFromCatching += (ModEntry.INSTANCE.config.CatchSpeed - 1f) * delta;
            lastDistanceFromCatching = distanceFromCatching;
            distanceFromCatchingField.SetValue(distanceFromCatching);

            // Speed warp on treasure catching
            float treasureCatchLevel = treasureCatchLevelField.GetValue();
            delta = treasureCatchLevel - this.lastTreasureCatchLevel;
            treasureCatchLevel += (ModEntry.INSTANCE.config.TreasureCatchSpeed - 1f) * delta;
            lastTreasureCatchLevel = treasureCatchLevel;
            treasureCatchLevelField.SetValue(treasureCatchLevel);

            bool perfect = perfectField.GetValue();
            bool treasure = treasureField.GetValue();
            bool treasureCaught = treasureCaughtField.GetValue();

            // Check if still perfect, otherwise apply changes to loot
            if (!perfectChanged && !perfect) {
                perfectChanged = true;
                fishQualityField.SetValue(Math.Min(this.origQuality, 1));
                int streak = FishHelper.getStreak(this.user);
                FishHelper.setStreak(this.user, 0);
                if (this.origStreak >= ModEntry.INSTANCE.config.StreakForIncreasedQuality) {
                    if (!treasure)
                        Game1.showGlobalMessage("You lost your perfect fishing streak of " + (this.origStreak) + ".");
                    else
                        Game1.showGlobalMessage("Catch the treasure and the fish to keep your streak of " + (this.origStreak) + "!");
                }
            }

            if (!treasureChanged && !perfect && treasure && treasureCaught) {
                treasureChanged = true;
                int qualityBonus = (int) Math.Floor((double) this.origStreak / ModEntry.INSTANCE.config.StreakForIncreasedQuality);
                int quality = this.origQuality;
                quality = Math.Min(quality + qualityBonus, 3);
                if (quality == 3) quality++;
                fishQualityField.SetValue(quality);
            }

            base.update(time);

            distanceFromCatching = distanceFromCatchingField.GetValue();

            if (distanceFromCatching <= 0.0) {
                // Failed to catch fish
                if (!notifiedFailOrSucceed && treasure) {
                    notifiedFailOrSucceed = true;
                    if (this.origStreak >= ModEntry.INSTANCE.config.StreakForIncreasedQuality)
                        Game1.showGlobalMessage("You lost your perfect fishing streak.");
                }
            } else if (distanceFromCatching >= 1.0) {
                // Succeeded in catching the fish
                if (!notifiedFailOrSucceed && treasureChanged && treasure && treasureCaught) {
                    notifiedFailOrSucceed = true;
                    if (this.origStreak >= ModEntry.INSTANCE.config.StreakForIncreasedQuality)
                        Game1.showGlobalMessage("You kept your perfect fishing streak!");
                    FishHelper.setStreak(this.user, this.origStreak);
                }
            }
        }
    }
}
