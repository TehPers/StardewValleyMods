using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using TehPers.Stardew.FishingOverhaul.Configs;
using TehPers.Stardew.Framework;
using static TehPers.Stardew.FishingOverhaul.Configs.ConfigTreasure;
using SFarmer = StardewValley.Farmer;

namespace TehPers.Stardew.FishingOverhaul {
    public class FishingRodOverrides {
        // Difficulty affects quality
        // Fish-specific lures (tackle)

        // Perfect catches increase fish/treasure quality
        // - Fish quality is increased by +1, but gold is only possible on perfect catches
        //    - Fish quality is further increased based on perfect catch streak
        // - Treasure quality is increased as your streak increases, but still obviously random
        // - Treasure is more likely to appear as your streak increases

        private static Dictionary<SFarmer, int> clearWaterDistances = new Dictionary<SFarmer, int>();

        public static void startMinigameEndFunction(FishingRod rod, int extra) {
            ModFishing.INSTANCE.Monitor.Log("Overriding fishing minigame", LogLevel.Trace);
            ConfigMain config = ModFishing.INSTANCE.config;
            SFarmer lastUser = ModFishing.INSTANCE.Helper.Reflection.GetPrivateValue<SFarmer>(rod, "lastUser");
            int clearWaterDistance = ModFishing.INSTANCE.Helper.Reflection.GetPrivateValue<int>(rod, "clearWaterDistance");
            Vector2 bobber = ModFishing.INSTANCE.Helper.Reflection.GetPrivateValue<Vector2>(rod, "bobber");

            rod.isReeling = true;
            rod.hit = false;
            switch (lastUser.FacingDirection) {
                case 1:
                    lastUser.FarmerSprite.setCurrentSingleFrame(48, 32000, false, false);
                    break;
                case 3:
                    lastUser.FarmerSprite.setCurrentSingleFrame(48, 32000, false, true);
                    break;
            }
            lastUser.FarmerSprite.pauseForSingleAnimation = true;
            clearWaterDistance = FishingRod.distanceToLand((int) (bobber.X / (double) Game1.tileSize - 1.0), (int) (bobber.Y / (double) Game1.tileSize - 1.0), lastUser.currentLocation);
            clearWaterDistances[lastUser] = clearWaterDistance;
            float num = 1f * (clearWaterDistance / 5f) * (Game1.random.Next(1 + Math.Min(10, lastUser.FishingLevel) / 2, 6) / 5f);
            if (rod.favBait)
                num *= 1.2f;
            float fishSize = Math.Max(0.0f, Math.Min(1f, num * (float) (1.0 + Game1.random.Next(-10, 10) / 100.0)));
            bool treasure = false;

            double treasureChance = config.TreasureChance + lastUser.LuckLevel * config.TreasureLuckLevelEffect + (rod.getBaitAttachmentIndex() == 703 ? config.TreasureBaitEffect : 0.0) + (rod.getBobberAttachmentIndex() == 693 ? config.TreasureBobberEffect : 0.0) + Game1.dailyLuck * config.TreasureDailyLuckEffect + (lastUser.professions.Contains(9) ? config.TreasureChance : 0.0) + config.TreasureStreakEffect * FishHelper.getStreak(lastUser);
            treasureChance = Math.Min(treasureChance, config.MaxTreasureChance);
            if (!Game1.isFestival() && lastUser.fishCaught != null && lastUser.fishCaught.Count > 1 && Game1.random.NextDouble() < treasureChance)
                treasure = true;

            // Override caught fish
            bool legendary = FishHelper.isLegendary(extra);
            if ((!legendary && !config.UseVanillaFish) || (legendary && config.OverrideLegendaries)) {
                int origExtra = extra;
                extra = FishHelper.getRandomFish(clearWaterDistance);
                if (FishHelper.isTrash(extra)) {
                    if (false) {
                        Game1.showGlobalMessage("No valid fish to catch! Giving junk instead.");
                        StardewValley.Object o = new StardewValley.Object(extra, 1, false, -1, 0);
                        rod.pullFishFromWater(extra, -1, 0, 0, false, false);
                        return;
                    } else {
                        ModFishing.INSTANCE.Monitor.Log("No valid fish to catch! Using original fish instead.", LogLevel.Warn);
                        extra = origExtra;
                    }
                }
            }

            // Show custom bobber bar
            Game1.activeClickableMenu = new CustomBobberBar(lastUser, extra, fishSize, treasure, rod.attachments[1] != null ? rod.attachments[1].ParentSheetIndex : -1, clearWaterDistance);
        }

        public static void openTreasureMenuEndFunction(FishingRod rod, int extra) {
            ModFishing.INSTANCE.Monitor.Log("Successfully replaced treasure", LogLevel.Trace);

            ConfigMain config = ModFishing.INSTANCE.config;
            SFarmer lastUser = ModFishing.INSTANCE.Helper.Reflection.GetPrivateValue<SFarmer>(rod, "lastUser");
            int clearWaterDistance = 5;
            if (config.OverrideFishing) {
                if (clearWaterDistances.ContainsKey(lastUser))
                    clearWaterDistance = clearWaterDistances[lastUser];
                else
                    ModFishing.INSTANCE.Monitor.Log("The bobber bar was not replaced. Fishing might not be overridden by this mod", LogLevel.Warn);
            }
            int whichFish = ModFishing.INSTANCE.Helper.Reflection.GetPrivateValue<int>(rod, "whichFish");
            int fishQuality = ModFishing.INSTANCE.Helper.Reflection.GetPrivateValue<int>(rod, "fishQuality");

            lastUser.gainExperience(5, 10 * (clearWaterDistance + 1));
            rod.doneFishing(lastUser, true);
            lastUser.completelyStopAnimatingOrDoingAction();

            // REWARDS
            List<Item> rewards = new List<Item>();
            if (extra == 1) rewards.Add(new StardewValley.Object(whichFish, 1, false, -1, fishQuality));

            List<TreasureData> possibleLoot = new List<ConfigTreasure.TreasureData>(config.PossibleLoot)
                .Where(treasure => treasure.isValid(lastUser.FishingLevel, clearWaterDistance)).ToList();

            // Select rewards
            float chance = 1f;
            int streak = FishHelper.getStreak(lastUser);
            while (possibleLoot.Count > 0 && rewards.Count < config.MaxTreasureQuantity && Game1.random.NextDouble() <= chance) {
                TreasureData treasure = possibleLoot.Choose(Game1.random);

                int id = treasure.id + Game1.random.Next(treasure.idRange - 1);

                if (id == Objects.LOST_BOOK) {
                    if (lastUser.archaeologyFound == null || !lastUser.archaeologyFound.ContainsKey(102) || lastUser.archaeologyFound[102][0] >= 21)
                        continue;
                    Game1.showGlobalMessage("You found a lost book. The library has been expanded.");
                }

                int count = Game1.random.Next(treasure.minAmount, treasure.maxAmount);

                Item reward;
                if (treasure.meleeWeapon) {
                    reward = new MeleeWeapon(id);
                } else if (id >= Ring.ringLowerIndexRange && id <= Ring.ringUpperIndexRange) {
                    reward = new Ring(id);
                } else if (id >= 504 && id <= 513) {
                    reward = new Boots(id);
                } else {
                    reward = new StardewValley.Object(Vector2.Zero, id, count);
                }

                rewards.Add(reward);
                if (!config.AllowDuplicateLoot || !treasure.allowDuplicates)
                    possibleLoot.Remove(treasure);

                //rewards.Add(new StardewValley.Object(Vector2.Zero, Objects.BAIT, Game1.random.Next(10, 25)));
            }

            // Add bait if no rewards were selected. NOTE: This should never happen
            if (rewards.Count == 0) {
                ModFishing.INSTANCE.Monitor.Log("Could not find any valid loot for the treasure chest. Check your treasure.json?", LogLevel.Warn);
                rewards.Add(new StardewValley.Object(685, Game1.random.Next(2, 5) * 5, false, -1, 0));
            }

            // Show rewards GUI
            Game1.activeClickableMenu = new ItemGrabMenu(rewards);
            (Game1.activeClickableMenu as ItemGrabMenu).source = 3;
            lastUser.completelyStopAnimatingOrDoingAction();
        }
    }
}
