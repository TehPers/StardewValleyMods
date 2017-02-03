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

namespace TehPers.Stardew.FishingOverhaul {
    public class FishingRodOverrides {
        // Difficulty affects quality
        // Fish-specific lures (tackle)

        // Perfect catches increase fish/treasure quality
        // - Fish quality is increased by +1, but gold is only possible on perfect catches
        //    - Fish quality is further increased based on perfect catch streak
        // - Treasure quality is increased as your streak increases, but still obviously random
        // - Treasure is more likely to appear as your streak increases

        public static void startMinigameEndFunction(FishingRod rod, int extra) {
            ConfigMain config = ModEntry.INSTANCE.config;
            Farmer lastUser = ModEntry.INSTANCE.Helper.Reflection.GetPrivateValue<Farmer>(rod, "lastUser");
            int clearWaterDistance = ModEntry.INSTANCE.Helper.Reflection.GetPrivateValue<int>(rod, "clearWaterDistance");
            Vector2 bobber = ModEntry.INSTANCE.Helper.Reflection.GetPrivateValue<Vector2>(rod, "bobber");

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
            float num = 1f * (clearWaterDistance / 5f) * (Game1.random.Next(1 + Math.Min(10, lastUser.FishingLevel) / 2, 6) / 5f);
            if (rod.favBait)
                num *= 1.2f;
            float fishSize = Math.Max(0.0f, Math.Min(1f, num * (float) (1.0 + Game1.random.Next(-10, 10) / 100.0)));
            bool treasure = false;

            double treasureChance = config.TreasureChance + lastUser.LuckLevel * config.TreasureLuckLevelEffect + (rod.getBaitAttachmentIndex() == 703 ? config.TreasureBaitEffect : 0.0) + (rod.getBobberAttachmentIndex() == 693 ? config.TreasureBobberEffect : 0.0) + Game1.dailyLuck * config.TreasureDailyLuckEffect + (lastUser.professions.Contains(9) ? config.TreasureChance : 0.0) + config.TreasureStreakEffect * CustomBobberBar.getStreak(lastUser);
            if (!Game1.isFestival() && lastUser.fishCaught != null && lastUser.fishCaught.Count > 1 && Game1.random.NextDouble() < treasureChance)
                treasure = true;
            Game1.activeClickableMenu = new CustomBobberBar(lastUser, extra, fishSize, treasure, rod.attachments[1] != null ? rod.attachments[1].ParentSheetIndex : -1, clearWaterDistance);
        }

        public static void openTreasureMenuEndFunction(FishingRod rod, int extra) {
            ModEntry.INSTANCE.Monitor.Log("Successfully replaced treasure", LogLevel.Trace);

            ConfigMain config = ModEntry.INSTANCE.config;
            Farmer lastUser = ModEntry.INSTANCE.Helper.Reflection.GetPrivateValue<Farmer>(rod, "lastUser");
            int clearWaterDistance = ModEntry.INSTANCE.Helper.Reflection.GetPrivateValue<int>(rod, "clearWaterDistance");
            int whichFish = ModEntry.INSTANCE.Helper.Reflection.GetPrivateValue<int>(rod, "whichFish");
            int fishQuality = ModEntry.INSTANCE.Helper.Reflection.GetPrivateValue<int>(rod, "fishQuality");

            lastUser.gainExperience(5, 10 * (clearWaterDistance + 1));
            rod.doneFishing(lastUser, true);
            lastUser.completelyStopAnimatingOrDoingAction();

            // REWARDS
            List<Item> rewards = new List<Item>();
            if (extra == 1) rewards.Add(new StardewValley.Object(whichFish, 1, false, -1, fishQuality));

            List<ConfigTreasure.TreasureData> possibleLoot = new List<ConfigTreasure.TreasureData>(config.PossibleLoot);
            possibleLoot.Sort((a, b) => a.chance.CompareTo(b.chance));

            // Select rewards
            float chance = 1f;
            while (config.PossibleLoot.Length > 0 && Game1.random.NextDouble() <= chance) {
                bool rewarded = false;
                foreach (ConfigTreasure.TreasureData treasure in possibleLoot) {
                    if (lastUser.FishingLevel >= treasure.minLevel && lastUser.FishingLevel <= treasure.maxLevel && clearWaterDistance >= treasure.minCastDistance) {
                        if (Game1.random.NextDouble() < treasure.chance * (CustomBobberBar.getStreak(lastUser) > 0 ? config.PerfectTreasureQualityMult : 1f)) {
                            int id = treasure.id + Game1.random.Next(treasure.idRange - 1);

                            if (id == Objects.LOST_BOOK) {
                                if (lastUser.archaeologyFound == null || !lastUser.archaeologyFound.ContainsKey(102) || lastUser.archaeologyFound[102][0] >= 21)
                                    continue;
                                Game1.showGlobalMessage("You found a lost book. The library has been expanded.");
                            }

                            int count = Game1.random.Next(treasure.minAmount, treasure.maxAmount);

                            Item reward;
                            if (id >= Ring.ringLowerIndexRange && id <= Ring.ringUpperIndexRange) {
                                reward = new Ring(id);
                            } else if (id >= 504 && id <= 513) {
                                reward = new Boots(id);
                            } else {
                                reward = new StardewValley.Object(Vector2.Zero, id, count);
                            }

                            rewards.Add(reward);
                            rewarded = true;
                            break;
                        }
                    }
                }

                if (rewarded)
                    chance *= config.AdditionalLootChance;
                //rewards.Add(new StardewValley.Object(Vector2.Zero, Objects.BAIT, Game1.random.Next(10, 25)));
            }

            // Add bait if no rewards were selected. NOTE: This should never happen
            if (rewards.Count == 0) rewards.Add(new StardewValley.Object(685, Game1.random.Next(2, 5) * 5, false, -1, 0));

            // Show rewards GUI
            Game1.activeClickableMenu = new ItemGrabMenu(rewards);
            (Game1.activeClickableMenu as ItemGrabMenu).source = 3;
            lastUser.completelyStopAnimatingOrDoingAction();
        }
    }
}
