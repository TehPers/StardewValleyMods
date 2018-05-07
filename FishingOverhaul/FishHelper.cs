using System;
using System.Collections.Generic;
using System.Linq;
using FishingOverhaul.Configs;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using TehCore.Api.Enums;
using TehCore.Api.Weighted;
using TehCore.Enums;
using TehCore.Helpers;
using SFarmer = StardewValley.Farmer;

namespace FishingOverhaul {
    internal class FishHelper {
        public static int? GetRandomFish(Farmer who, int mineLevel = -1) => FishHelper.GetRandomFish(ModFishing.Instance.Api.GetPossibleFish(who));

        public static int? GetRandomFish(IEnumerable<IWeightedElement<int?>> possibleFish) {
            possibleFish = possibleFish.ToList();

            // No possible fish
            if (!possibleFish.Any())
                return null;

            // Select a fish
            return possibleFish.Choose(Game1.random);
        }

        public static int GetRandomTrash() => ModFishing.Instance.Api.GetPossibleTrash().Choose(Game1.random);

        public static bool IsTrash(int id) => ModFishing.Instance.Api.GetPossibleTrash().Any(t => t.Value == id);

        public static bool IsLegendary(int fish) => fish == 159 || fish == 160 || fish == 163 || fish == 682 || fish == 775;

        public static float GetRawFishChance(SFarmer who) {
            ConfigMain.ConfigGlobalFish config = ModFishing.Instance.MainConfig.GlobalFishSettings;

            // Calculate chance
            float chance = config.FishBaseChance;
            chance += who.FishingLevel * config.FishLevelEffect;
            chance += who.LuckLevel * config.FishLuckLevelEffect;
            chance += (float) Game1.dailyLuck * config.FishDailyLuckEffect;
            chance += ModFishing.Instance.Api.GetStreak(who) * config.FishStreakEffect;

            return chance;
        }

        public static float GetRawTreasureChance(SFarmer who, FishingRod rod) {
            ConfigMain.ConfigGlobalTreasure config = ModFishing.Instance.MainConfig.GlobalTreasureSettings;

            // Calculate chance
            float chance = config.TreasureChance;
            chance += who.LuckLevel * config.TreasureLuckLevelEffect;
            chance += (float) Game1.dailyLuck * config.TreasureDailyLuckEffect;
            chance += config.TreasureStreakEffect * ModFishing.Instance.Api.GetStreak(who);
            if (rod.getBaitAttachmentIndex() == 703)
                chance += config.TreasureBaitEffect;
            if (rod.getBobberAttachmentIndex() == 693)
                chance += config.TreasureBobberEffect;
            if (who.professions.Contains(9))
                chance += config.TreasureChance;

            return Math.Min(chance, config.MaxTreasureChance);
        }

        public static float GetRawUnawareChance(SFarmer who) {
            ConfigMain.ConfigUnaware config = ModFishing.Instance.MainConfig.UnawareSettings;

            // Calculate chance
            float chance = config.UnawareChance;
            chance += who.LuckLevel * config.UnawareLuckLevelEffect;
            chance += (float) Game1.dailyLuck * config.UnawareDailyLuckEffect;

            return chance;
        }
    }
}
